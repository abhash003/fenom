using System;
using System.Text;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class ProvisioningView : BaseContentPage
    {
        private ProvisioningViewModel model;

        public ProvisioningView()
        {
            InitializeComponent();
            BindingContext = model = new ProvisioningViewModel();
            Title = "Provisioning";
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
        protected override void NewGlobalData()
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
            Services.BleHub.SendSerailNumber(model.SerialNumber);
            Services.BleHub.SendDateTime(DateTime.Now);
            Services.Cache.DeviceSerialNumber = string.Format("F150-{0}", model.SerialNumber);
        }
    }
}
