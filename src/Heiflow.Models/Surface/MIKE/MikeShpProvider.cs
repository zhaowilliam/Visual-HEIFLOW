﻿//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using DotSpatial.Data;
using Heiflow.Core.IO;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using DotNetDBF;
using GeoAPI.Geometries;

namespace Heiflow.Models.Surface.MIKE
{
    public class MikeShpProvider
    {
        public MikeShpProvider()
        {

        }

        /// <summary>
        /// Import Mike MESH file from existing SHP files
        /// </summary>
        /// <param name="node_shp"></param>
        /// <param name="element_shp"></param>
        /// <param name="meshfile"></param>
        public void Import(string node_shp, string element_shp, string meshfile)
        {
            StreamWriter sw = new StreamWriter(meshfile);
            var node_fs = FeatureSet.Open(node_shp);
            var element_fs = FeatureSet.Open(element_shp);
            int nelem = element_fs.DataTable.Rows.Count;
            int nnode = node_fs.DataTable.Rows.Count;
            int id = 0;
            string line = nelem + " 1000 " + nnode + " WGS84";
            sw.WriteLine(line);

            var node_dt = node_fs.DataTable;
            var elem_dt = element_fs.DataTable;
            Coordinate[] list = new Coordinate[nnode];

            foreach (var f in node_fs.Features)
            {
                int npt = f.Geometry.Coordinates.Length;
                for (int n = 0; n < npt; n++)
                {
                    line = string.Format("{0} {1} {2} {3} 1", id + 1, f.Geometry.Coordinates[n].X, f.Geometry.Coordinates[n].Y, node_dt.Rows[id]["Bathymetry"].ToString());
                    sw.WriteLine(line);
                    f.Geometry.Coordinates[n].M = id;
                    list[id] = f.Geometry.Coordinates[n];
                    id++;
                }
            }

            line = nelem + " 4 25";
            sw.WriteLine(line);
            var vertices = element_fs.Vertex;
            var nshpx = element_fs.ShapeIndices.Count;
            for (int n = 0; n < nshpx; n++)
            {
                var shpx = element_fs.ShapeIndices[n];
                int start = shpx.Parts[0].StartIndex;
                int end = shpx.Parts[shpx.Parts.Count - 1].EndIndex;
                int npt = end - start + 1;
                line = (n + 1).ToString() + " ";
                for (int i = start; i < start + 3; i++)
                {
                    var xx = vertices[2 * i];
                    var yy = vertices[2 * i + 1];
                    var iid = (from p in list where p.X == xx && p.Y == yy select p).First().M + 1;
                    line += iid + " ";
                }
                line += " 0";
                sw.WriteLine(line);
            }
            node_fs.Close();
            element_shp.Clone();
            sw.Close();
        }

        /// <summary>
        /// Export Mike SHP Results to AC file
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="ac_filename"></param>
        /// <param name="shpfiles"></param>
        /// <param name="field"></param>
        public void Export(ITriangularGrid grid, string ac_filename, string[] shpfiles, string field)
        {
            int steps = shpfiles.Length;
            int nfeature = grid.VertexCount;
            var buf = new DataCube<float>(1, steps, nfeature);

            for (int i = 0; i < steps; i++)
            {
                var fs = FeatureSet.Open(shpfiles[i]);
                var vec = (from dr in fs.DataTable.AsEnumerable() select (float)dr.Field<double>(field)).ToArray();
                for (int k = 0; k < grid.VertexCount; k++)
                {
                    var cells = grid.Topology.NodeConnectedCells[k];
                    float temp = 0;
                    for (int c = 0; c < cells.Length; c++)
                    {
                        temp += vec[cells[c]];
                    }
                    temp /= cells.Length;
                    buf[0, i, k] = temp;
                }
                fs.Close();
            }
            buf.Variables = new string[] { field };
            DataCubeStreamWriter ac = new DataCubeStreamWriter(ac_filename);
            ac.WriteAll(buf);
        }

        public void Export(ITriangularGrid grid, string ac_filename, string[] dbffiles, int[] index)
        {
            int steps = dbffiles.Length;
            int nfeature = grid.VertexCount;
            var buf = new DataCube<float>(index.Length, steps, nfeature);
            string[] field = new string[index.Length];
            for (int i = 0; i < steps; i++)
            {
                DBFReader dbf = new DBFReader(dbffiles[i]);
                var vec = new double[index.Length][];
                for (int t = 0; t < index.Length; t++)
                {
                    vec[t] = new double[dbf.RecordCount];
                }
                for (int n = 0; n < dbf.RecordCount; n++)
                {
                    var obj = dbf.NextRecord();
                    for (int t = 0; t < index.Length; t++)
                    {
                        vec[t][n] = double.Parse(obj[index[t]].ToString());
                    }
                }

                for (int t = 0; t < index.Length; t++)
                {
                    for (int k = 0; k < grid.VertexCount; k++)
                    {
                        var cells = grid.Topology.NodeConnectedCells[k];
                        double temp = 0;
                        for (int c = 0; c < cells.Length; c++)
                        {
                            temp += vec[t][cells[c]];
                        }
                        temp /= cells.Length;
                        buf[t, i, k] = (float)temp;
                    }
                    field[t] = dbf.Fields[index[t]].Name;
                }
                dbf.Close();
            }
            buf.Variables = field;
            DataCubeStreamWriter ac = new DataCubeStreamWriter(ac_filename);    
            ac.WriteAll(buf);
        }

        public void Export(string ac_filename, string[] dbffiles, int[] index)
        {
            int steps = dbffiles.Length;
            DBFReader dbf = new DBFReader(dbffiles[0]);
            int nfeature = dbf.RecordCount;
            var buf = new DataCube<float>(index.Length, steps, nfeature);
            string[] field = new string[index.Length];
            for (int i = 0; i < index.Length; i++)
            {
                field[i] = dbf.Fields[index[i]].Name;
            }
              
            dbf.Close();

            for (int t = 0; t < steps; t++)
            {
                dbf = new DBFReader(dbffiles[t]);
                for (int n = 0; n < dbf.RecordCount; n++)
                {
                    var obj = dbf.NextRecord();
                    for (int i = 0; i < index.Length; i++)
                    {
                        buf[i, t, n] = float.Parse(obj[index[i]].ToString());
                    }
                }
                dbf.Close();
            }
            buf.Variables = field;
            DataCubeStreamWriter ac = new DataCubeStreamWriter(ac_filename);
            ac.WriteAll(buf);
        }
    }
}
