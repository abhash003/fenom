using FenomPlus.ViewModels;
using System;

namespace FenomPlus.Views
{
    public partial class CalibrationView : BaseContentPage
    {
        private CalibrationViewModel model;

        public CalibrationView()
        {
            InitializeComponent();
            BindingContext = model = new CalibrationViewModel();
            Title = "Calibration";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSendClicked(System.Object sender, System.EventArgs e)
        {
            Services.BleHub.SendCalibration(
                Convert.ToDouble(model.CalibrationValue1),
                Convert.ToDouble(model.CalibrationValue2),
                Convert.ToDouble(model.CalibrationValue3));
        }
    }
}
