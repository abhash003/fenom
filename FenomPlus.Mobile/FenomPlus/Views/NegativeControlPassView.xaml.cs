using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class NegativeControlPassView : BaseContentPage
    {
        private NegativeControlPassViewModel model;

        public NegativeControlPassView()
        {
            InitializeComponent();
            BindingContext = model = new NegativeControlPassViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancel(object sender, EventArgs e)
        {
            await Services.Navigation.QualityControlView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnNext(System.Object sender, System.EventArgs e)
        {
            await Services.Navigation.HumanControlPerformingView();
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
