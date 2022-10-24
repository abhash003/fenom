﻿using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class HumanControlPerformingView : BaseContentPage
    {
        private HumanControlPerformingViewModel model;

        public HumanControlPerformingView()
        {
            InitializeComponent();
            BindingContext = model = new HumanControlPerformingViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnCancel(System.Object sender, System.EventArgs e)
        {
            await Services.Navigation.QualityControlView();
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
