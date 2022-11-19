using FenomPlus.Controls;
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
    public partial class StatusDeviceHubView : BaseContentPage
    {
        private readonly StatusViewModel StatusViewModel;

        public StatusDeviceHubView()
        {
            InitializeComponent();
            BindingContext = StatusViewModel = new StatusViewModel();

            SensorInfo.BindingContext = StatusViewModel.SensorInfoViewModel;
            DeviceInfo.BindingContext = StatusViewModel.DeviceInfoViewModel;
            QualityControlInfo.BindingContext = StatusViewModel.QualityControlInfoViewModel;
            BluetoothInfo.BindingContext = StatusViewModel.BluetoothInfoViewModel;
            HumidityInfo.BindingContext = StatusViewModel.HumidityInfoViewModel;
            PressureInfo.BindingContext = StatusViewModel.PressureInfoViewModel;
            TemperatureInfo.BindingContext = StatusViewModel.TemperatureInfoViewModel;
            BatteryInfo.BindingContext = StatusViewModel.BatteryInfoViewModel;
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
    }
}