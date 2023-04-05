using FenomPlus.Services;
using FenomPlus.ViewModels;
using FenomPlus.ViewModels.QualityControl;
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
    public partial class QCNegativeControlTestView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCNegativeControlTestView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MarigoldProgressWheel.StartAnimation();
        }

        protected override void OnDisappearing()
        {
            MarigoldProgressWheel.StopAnimation();
            base.OnDisappearing();
        }
    }
}