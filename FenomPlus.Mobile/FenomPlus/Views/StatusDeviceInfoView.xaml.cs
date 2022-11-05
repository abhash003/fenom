using FenomPlus.ViewModels;
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
    public partial class StatusDeviceInfoView : BaseContentPage
    {
        private StatusDeviceInfoViewModel StatusDeviceInfoViewModel;

        public StatusDeviceInfoView()
        {
            InitializeComponent();
            BindingContext = StatusDeviceInfoViewModel = new StatusDeviceInfoViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StatusDeviceInfoViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StatusDeviceInfoViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            StatusDeviceInfoViewModel.NewGlobalData();
        }
    }
}