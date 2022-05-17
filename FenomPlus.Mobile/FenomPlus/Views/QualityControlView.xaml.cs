﻿using System;
using System.Collections.Generic;
using FenomPlus.Helpers;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class QualityControlView : BaseContentPage
    {
        private QualityControlViewModel model;

        public QualityControlView()
        {
            InitializeComponent();
            BindingContext = model = new QualityControlViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
        }
    }
}
