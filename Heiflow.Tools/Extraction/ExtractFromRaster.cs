﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.DataManagement
{
    public class ExtractFromRaster : MapLayerRequiredTool
    {
        private IFeatureSet _target_layer;
        private IRaster _dem_layer;
        public ExtractFromRaster()
        {
            Name = "Extract From Raster";
            Category = "Extraction";
            Description = "Extract values from a Raster layer  to a data cube";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("Raster")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor RasterLayer
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The data cube to which the raster values will be extracted. The data cube style should be [0][0][:]")]
        public string Matrix
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("Stream layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor TargetFeature
        {
            get;
            set;
        }

        public override void Initialize()
        {
            if (TargetFeature == null || RasterLayer == null)
            {
                Initialized = false;
                return;
            }
            _target_layer = TargetFeature.DataSet as IFeatureSet;
            _dem_layer = RasterLayer.DataSet as IRaster;
            if (_target_layer == null || _dem_layer == null)
            {
                Initialized = false;
                return;
            }
            this.Initialized = _target_layer.FeatureType == FeatureType.Polygon||  _target_layer.FeatureType == FeatureType.Point;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int progress = 0;
            int count = 1;
            if (_target_layer != null )
            {
                var nrow = _target_layer.NumRows();
                var dx = System.Math.Sqrt(_target_layer.GetFeature(0).Geometry.Area);
                int nsample = (int)System.Math.Floor(dx / _dem_layer.CellHeight);
                var mat = new My3DMat<float>(1, 1, nrow);
                mat.Name = Matrix;
                mat.Variables = new string[] { Matrix };
                mat.TimeBrowsable = false;
                mat.AllowTableEdit = true;
                List<Coordinate> list = new List<Coordinate>();
                if (_target_layer.FeatureType == FeatureType.Polygon)
                {
                    for (int i = 0; i < nrow; i++)
                    {
                        float sum_cellv = 0;
                        int npt = 0;
                        list.Clear();
                        var fea = _target_layer.GetFeature(i).Geometry;
                        var x0 = (from p in fea.Coordinates select p.X).Min();
                        var y0 = (from p in fea.Coordinates select p.Y).Min();
                        for (int r = 0; r <= nsample; r++)
                        {
                            var y = y0 + r * _dem_layer.CellHeight;
                            for (int c = 0; c <= nsample; c++)
                            {
                                var x = x0 + c * _dem_layer.CellWidth;
                                Coordinate pt = new Coordinate(x, y);
                                list.Add(pt);
                            }
                        }
                        foreach (var pt in list)
                        {
                             var cell = _dem_layer.ProjToCell(pt);
                             if (cell != null && cell.Row > 0)
                             {
                                 var buf = (float)_dem_layer.Value[cell.Row, cell.Column]; 
                                 if(buf != _dem_layer.NoDataValue)
                                 {
                                     sum_cellv += buf;
                                     npt++;
                                 }
                             }
                        }
                        if (npt > 0)
                            sum_cellv = sum_cellv / npt;
                        mat.Value[0][0][i] = sum_cellv;

                        progress = i * 100 / nrow;
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing polygon: " + i);
                            count++;
                        }
                    }
                }
                else
                {
                    Coordinate[] coors = new Coordinate[nrow];

                    for (int i = 0; i < nrow; i++)
                    {
                        var geo_pt = _target_layer.GetFeature(i).Geometry;
                        coors[i] = geo_pt.Coordinate;
                    }

                    for (int i = 0; i < nrow; i++)
                    {
                        var cell = _dem_layer.ProjToCell(coors[i]);
                        if (cell != null && cell.Row > 0)
                        {
                            mat.Value[0][0][i] = (float)_dem_layer.Value[cell.Row, cell.Column];
                        }
                        progress = i * 100 / nrow;
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing point: " + i);
                            count++;
                        }
                    }
                }
                Workspace.Add(mat);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: the input layers are incorrect.");
                return false;
            }
        }
    }
}