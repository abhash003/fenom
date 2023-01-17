using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;
using FenomPlus.Enums.ErrorCodes;

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
            int statusCode = Services.Cache.BreathManeuver.StatusCode;

            var error = ErrorCodeLookup.Lookup(statusCode);
            ErrorCode = error.Code;
            ErrorMessage = error.Message;
        }
    }
}
