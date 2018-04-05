﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Controls;
  using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Animation;
using Heiflow.Presentation.Controls;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using Heiflow.Core.Data;
using Heiflow.Presentation.Animation;
using System.ComponentModel.Composition;
using Heiflow.Presentation;
using Heiflow.Presentation.Services;
using Heiflow.Applications;
using BrightIdeasSoftware;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;

namespace Heiflow.Controls.WinForm.Controls
{

    public partial class AnimationPlayer : UserControl, IAnimationView,IChildView
    {
        private IDataCubeWorkspace _WorkSpace;
        private bool isPlay = false;
        private IDataCubeObject _selectedDc;
        public delegate void ControlStringConsumer(ComboBox control, List<ITimeService> list);  // defines a delegate type
        private List<IDataCubeAnimation> _Animators = new List<IDataCubeAnimation>();
        private IDataCubeAnimation _SelectedAnimator;
        private IShellService _ShellService;

        public AnimationPlayer()
        {
            InitializeComponent();
            _WorkSpace = new DataCubeWorkspace();
            _WorkSpace.DataSourceCollectionChanged += _WorkSpace_DataSourceCollectionChanged;
            olvDataCubeTree.RootKeyValue = 9999;
            this.Load += AnimationPlayer_Load;

            olvDataCubeTree.UseTranslucentHotItem = true;
            olvDataCubeTree.SmallImageList = imageList1;
            olvColumnState.ImageAspectName = "ImageName";
        }
        public object DataContext
        {
            get;
            set;
        }

        public IDataCubeWorkspace DataCubeWorkspace
        {
            get
            {
                return _WorkSpace;
            }
        }
        public string ChildName
        {
            get { return "AnimationPlayer"; }
        }

        public void ShowView(IWin32Window pararent)
        {
             
        }
        private void AnimationPlayer_Load(object sender, EventArgs e)
        {
            if (_ShellService == null)
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();

            MapAnimation map = new MapAnimation();
            map.CurrentChanged += map_CurrentChanged;
            _Animators.Add(map);
            SurfaceAnimation surf = new SurfaceAnimation()
            {
                SurfacePlot = _ShellService.SurfacePlot
            };
            surf.CurrentChanged += map_CurrentChanged;
            _Animators.Add(surf);
            cmbAnimators.ComboBox.DisplayMember = "Name";
            cmbAnimators.ComboBox.DataSource = _Animators.ToArray();
            cmbAnimators.ComboBox.SelectedIndex = 0;
        }
        private void _WorkSpace_DataSourceCollectionChanged(object sender, EventArgs e)
        {
            var dt = _WorkSpace.ToDataTable();
            DataSet ds = new DataSet();
            dt.TableName = "DataCubes";
            ds.Tables.Add(dt);
            this.olvDataCubeTree.DataMember = "DataCubes";
            this.olvDataCubeTree.DataSource = new DataViewManager(ds);
            olvDataCubeTree.ExpandAll();
        }
        private void map_CurrentChanged(object sender, int e)
        {
            listBox_timeline.SelectedIndex = e;
        }

        private void olvDataCubeTree_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var dv = olvDataCubeTree.SelectedObject as DataRowView;
            if (dv != null)
            {
                var dr = dv.Row;
                var parent = int.Parse(dr["ParentID"].ToString());
                if (parent != 9999)
                {
                    _selectedDc = dr["DataCubeObject"] as IDataCubeObject;
                    _selectedDc.SelectedVariableIndex = int.Parse(dr["ID"].ToString());
                    if (_selectedDc.IsAllocated(_selectedDc.SelectedVariableIndex))
                    {
                        listBox_timeline.DataSource = _selectedDc.DateTimes;
                        listBox_timeline.SelectedIndex = 0;
                    }
                    else
                    {
                        listBox_timeline.DataSource = null;
                        listBox_timeline.Items.Clear();
                    }

                }
            }
            else
            {
                listBox_timeline.DataSource = null;
                listBox_timeline.Items.Clear();
            }
        }

        private void cmbAnimators_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedAnimator = cmbAnimators.SelectedItem as IDataCubeAnimation;
            if(cmbAnimators.SelectedIndex == 1)
                _ShellService.SelectPanel(DockPanelNames.View3DPanel);
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (isPlay)
            {
                _SelectedAnimator.Stop();
                isPlay = false;
                btnPlay.Image = Resources.GenericBlueRightArrowNoTail32;
            }
            else
            {
                btnPlay.Image = Resources.GenericBluePause32;
                isPlay = true;
                _SelectedAnimator.Play();
            }
        }


        private void listBoxTimeLine_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.Yellow);//Choose the color

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString(listBox_timeline.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        private void listBoxTimeLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_SelectedAnimator != null && _selectedDc != null && !isPlay)
            {
                _SelectedAnimator.DataSource = _selectedDc;
                _SelectedAnimator.Go(listBox_timeline.SelectedIndex);
            }
        }



        public void ClearContent()
        {
            
        }
        public void InitService()
        {

        }
    }

}
