using System;
using System.Collections.Generic;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class NegativeControlPerformView : BaseContentPage
    {
        private NegativeControlPerformViewModel model;

        public NegativeControlPerformView()
        {
            InitializeComponent();
            BindingContext = model = new NegativeControlPerformViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnCancel(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(QualityControlView)}"), false);
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
