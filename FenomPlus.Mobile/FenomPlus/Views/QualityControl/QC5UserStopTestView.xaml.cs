using FenomPlus.ViewModels.QualityControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FenomPlus.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FenomPlus.Services;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserStopTestView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCUserStopTestView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            QualityControlViewModel.InitUserStopBreathTest();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //QualityControlViewModel.OnDisappearing();
        }
    }
}