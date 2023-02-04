using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;
using FenomPlus.Enums.ErrorCodes;
using System.ComponentModel;

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


        private void UpdateError()
        {
            var bm = Services.DeviceService.Current.BreathManeuver;

            int statusCode = 0;
            if ((bm.TimeRemaining == 0 || bm.TimeRemaining == 0xfe) && bm.StatusCode != 0)
            {
                statusCode = bm.StatusCode;
            }
            else
            {
                statusCode = Services.DeviceService.Current.LastErrorCode;
            }

            var error = ErrorCodeLookup.Lookup(statusCode);

            ErrorCode = error.Code;
            ErrorMessage = error.Message;
        }
    }
}
