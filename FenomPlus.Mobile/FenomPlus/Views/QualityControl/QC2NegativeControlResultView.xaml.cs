using FenomPlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FenomPlus.ViewModels.QualityControl;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FenomPlus.Services;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCNegativeControlResultView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCNegativeControlResultView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //QualityControlViewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //QualityControlViewModel.OnDisappearing();
        }
    }
}