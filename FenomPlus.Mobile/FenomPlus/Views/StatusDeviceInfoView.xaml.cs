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
        private StatusDeviceInfoViewModel model;

        public StatusDeviceInfoView()
        {
            InitializeComponent();
            BindingContext = model = new StatusDeviceInfoViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();

            // This line was causing problems with Status from menu showing page
            //model.NewGlobalData();
        }
    }
}