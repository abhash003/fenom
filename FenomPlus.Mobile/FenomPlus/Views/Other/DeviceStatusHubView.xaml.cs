using FenomPlus.Controls;
using FenomPlus.Services;
using FenomPlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Svg;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceStatusHubView : BaseContentPage
    {
        private readonly StatusViewModel StatusViewModel;

        public DeviceStatusHubView()
        {
            InitializeComponent();
            BindingContext = StatusViewModel = AppServices.Container.Resolve<StatusViewModel>();

            SensorInfo.BindingContext = StatusViewModel.SensorViewModel;
            DeviceInfo.BindingContext = StatusViewModel.DeviceViewModel;
            QualityControlInfo.BindingContext = StatusViewModel.QualityControlViewModel;
            BluetoothInfo.BindingContext = StatusViewModel.BluetoothViewModel;
            HumidityInfo.BindingContext = StatusViewModel.HumidityViewModel;
            PressureInfo.BindingContext = StatusViewModel.PressureViewModel;
            TemperatureInfo.BindingContext = StatusViewModel.TemperatureViewModel;
            BatteryInfo.BindingContext = StatusViewModel.BatteryViewModel;

            //DetailsPopup.BindingContext = StatusViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StatusViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StatusViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            StatusViewModel.NewGlobalData();
        }

        private void DeviceInfo_OnClicked(object sender, EventArgs e)
        {
            
        }

        private void SensorInfo_OnClicked(object sender, EventArgs e)
        {
           
        }
    }
}