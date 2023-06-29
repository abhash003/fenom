using System;
using FenomPlus.Services;
using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserTestErrorView : BaseContentPage
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCUserTestErrorView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = AppServices.Container.Resolve<QualityControlViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (IsExitVisible)
                Services.Cache.TestType = Enums.TestTypeEnum.None;
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
