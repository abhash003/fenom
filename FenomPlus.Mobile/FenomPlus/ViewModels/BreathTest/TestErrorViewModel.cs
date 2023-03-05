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
            if (Services.DeviceService.Current == null) return;

            var bm = Services.DeviceService.Current.BreathManeuver;
            var esi = Services.DeviceService.Current.ErrorStatusInfo;

            int statusCode = 0;
            if ((bm.TimeRemaining == 0 || bm.TimeRemaining == 0xfe) && esi.ErrorCode != 0)
            {
                statusCode = esi.ErrorCode;
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
