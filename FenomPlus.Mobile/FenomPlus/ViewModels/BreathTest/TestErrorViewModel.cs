using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;

namespace FenomPlus.ViewModels
{
    public partial class TestErrorViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _errorCode;

        public TestErrorViewModel()
        {
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            UpdateError();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }

        private void UpdateError()
        {
            int statusCode = (Services.DeviceService.Current.BreathManeuver.StatusCode >= ErrorCodesEnum.code.Length) ?
                ErrorCodesEnum.code.Length :
                Services.DeviceService.Current.BreathManeuver.StatusCode;

            ErrorCode = ErrorCodesEnum.code[statusCode];
            ErrorMessage = ErrorCodesEnum.title[statusCode];
        }
    }
}
