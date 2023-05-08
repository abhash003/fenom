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
    public partial class QCUserTestResultView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCUserTestResultView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            QualityControlViewModel.InitUserTestResults();
        }

        protected override void OnDisappearing()
        {
            Services.Cache.TestType = Enums.TestTypeEnum.None;
            base.OnDisappearing();
            //QualityControlViewModel.OnDisappearing();
        }
    }
}