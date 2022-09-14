using System;
using System.Collections.Generic;
using FenomPlus.Helpers;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class ViewPastResultsView : BaseContentPage
    {
        private ViewPastResultsViewModel model;

        public ViewPastResultsView()
        {
            InitializeComponent();
            BindingContext = model = new ViewPastResultsViewModel();
            dataGrid.GridStyle = new CustomGridStyle();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }
    }
}
