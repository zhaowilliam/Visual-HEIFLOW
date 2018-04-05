﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using Heiflow.Image.ImageSets;
using Heiflow.Image.Recognization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Heiflow.Image
{
    public partial class RecgnizationForm : Form
    {
        private IImageSource _IImageSource;
        private RecognizerFactory _RecognizerFactory;
        private ColorClassifier _ColorClassification;
        private IRecognizer _Recognizer;
        private IImageSetsBuilder _Builder;
        public RecgnizationForm()
        {
            InitializeComponent();
            _IImageSource = new ImageSets.ImageSource();
            imageSourceManager1.ImageSource = _IImageSource;
            imageSourceManager1.TrainingImageDoubleClicked += imageSourceManager1_TrainingImageDoubleClicked;
            imageSourceManager1.RecgonizingImageDoubleClicked += imageSourceManager1_RecgonizingImageDoubleClicked;

            _Builder = new BitmapSetsBuilder();
            _ColorClassification = new ColorClassifier();
            _ColorClassification.KnownColors = new Color[1] { Color.Red };
            _ColorClassification.KnownValues = new double[1] { 1 };
            _ColorClassification.Update();

            _RecognizerFactory = new RecognizerFactory(_Builder,_ColorClassification);
            _RecognizerFactory.Initialize();

            cmb_Models.ComboBox.DisplayMember = "Name";
            cmb_Models.ComboBox.DataSource = _RecognizerFactory.Models;
            cmb_Models.SelectedIndex = 1;
        }

        private void openTraningImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "jpeg file|*.jpg|png file|*.png|bitmap file|*.bmp";
            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _IImageSource.AddTrainingImages(dlg.FileNames);
                imageSourceManager1.RefreshTree();
            }
        }

        private void openImagesToBeRecognizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "jpeg file|*.jpg|png file|*.png|bitmap file|*.bmp";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _IImageSource.AddRecoginzingImages(dlg.FileNames);
                imageSourceManager1.RefreshTree();
            }
        }


        private void openProjToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveProjToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void imageSourceManager1_RecgonizingImageDoubleClicked(object sender, string e)
        {
            pb_recg1.ImageLocation = e;
        }

        private void imageSourceManager1_TrainingImageDoubleClicked(object sender, string e)
        {
            pb_train1.ImageLocation = e;

        }

        private void cmb_Models_SelectedIndexChanged(object sender, EventArgs e)
        {
            var model = cmb_Models.SelectedItem as IForecastingModel;
            this.propertyGrid1.SelectedObject = model.Parameter;
            _Recognizer = _RecognizerFactory.Select(model.Name);
        }

        private void btnTrain_Click(object sender, EventArgs e)
        {
            if (_Recognizer != null)
            {
                var source = _IImageSource.ToTrainingImages();
                _Recognizer.Train(source[0], source[1]);
            }
        }

        private void btnRecognize_Click(object sender, EventArgs e)
        {
            //_Recognizer.Recognize(_Recognizer.)
        }

        private void webMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebMapForm webmap = new WebMapForm();
            webmap.Show();
        }

    }
}
