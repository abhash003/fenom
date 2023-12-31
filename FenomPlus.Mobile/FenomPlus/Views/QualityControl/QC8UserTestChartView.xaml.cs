﻿using FenomPlus.Services;
using FenomPlus.ViewModels;
using FenomPlus.ViewModels.QualityControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserTestChartView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCUserTestChartView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //QualityControlViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //QualityControlViewModel.OnDisappearing();
        }
    }
}