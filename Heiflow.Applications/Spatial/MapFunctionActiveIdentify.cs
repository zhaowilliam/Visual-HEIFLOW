﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Data;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Applications.Spatial
{
    public class MapFunctionActiveIdentify : MapFunction
    {
        private bool _standBy;
        private AppManager _AppManager;
        private VHFAppManager _vhf;

        public MapFunctionActiveIdentify(AppManager app, VHFAppManager vhf)
            : base(app.Map)
        {
            _AppManager = app;
            _vhf = vhf;
            Configure();
        }

        public IFeatureSet Grid
        {
            get;
            set;
        }

        private void Configure()
        {
            YieldStyle = YieldStyles.LeftButton | YieldStyles.Scroll;        
            Control map = Map as Control;
            if (map != null) map.MouseLeave += map_MouseLeave;
            this.Name = "MapFunctionActiveIdentify";
        }

        private void map_MouseLeave(object sender, EventArgs e)
        {
            Map.Invalidate();
        }

        protected override void OnMouseUp(GeoMouseArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            Rectangle rtol = new Rectangle(e.X - 8, e.Y - 8, 16, 16);
            Rectangle rstr = new Rectangle(e.X - 1, e.Y - 1, 2, 2);
            Extent tolerant = e.Map.PixelToProj(rtol);
            Extent strict = e.Map.PixelToProj(rstr);
            var _chart = _vhf.ShellService.WinChart;
            var data_service = _vhf.ProjectController.ActiveDataService;

            if (Grid != null && _chart != null && data_service.Source != null)
            {
                var selected = Grid.Select(strict, out tolerant);
                if (selected.Count > 0)
                {
                    var fea = selected[0];
                    var hru = int.Parse(fea.DataRow["HRU_ID"].ToString());
                    int ntime = data_service.Source.Size[1];
                    float[] yy = new float[ntime];
                    for (int i = 0; i < ntime; i++)
                    {
                        yy[i] = data_service.Source.Value[data_service.Source.SelectedVariableIndex][i][hru - 1];
                    }
                    _chart.Plot<float>(data_service.Source.DateTimes, yy, "HRU_" + hru.ToString(), SeriesChartType.FastLine);
                }
            }
        }

        /// <summary>
        /// Forces this function to begin collecting points for building a new shape.
        /// </summary>
        protected override void OnActivate()
        {
            if (_standBy == false)
            {
            }
            _vhf.ShellService.WinChart.ShowView(_vhf.ShellService.MainForm);

            _standBy = false;
            base.OnActivate();
        }

        /// <summary>
        /// Allows for new behavior during deactivation.
        /// </summary>
        protected override void OnDeactivate()
        {
            if (_standBy)
            {
                return;
            }
            // Don't completely deactivate, but rather go into standby mode
            // where we draw only the content that we have actually locked in.
            _standBy = true;
            _vhf.ShellService.WinChart.CloseView();
            Map.Invalidate();
        }

    }
}
