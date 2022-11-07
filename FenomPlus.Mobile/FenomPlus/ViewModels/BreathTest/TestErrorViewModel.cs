using FenomPlus.Enums;

namespace FenomPlus.ViewModels
{
    public class TestErrorViewModel : BaseViewModel
    {
        public TestErrorViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            UpdateError();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateError()
        {
            int statusCode = (Cache.BreathManeuver.StatusCode >= ErrorCodesEnum.code.Length) ?
                ErrorCodesEnum.code.Length :
                Cache.BreathManeuver.StatusCode;

            ErrorCode = ErrorCodesEnum.code[statusCode];
            ErrorMessage = ErrorCodesEnum.title[statusCode];
        }

        /// <summary>
        /// 
        /// </summary>
        private string _ErrorMessage;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _ErrorCode;
        public string ErrorCode
        {
            get => _ErrorCode;
            set
            {
                _ErrorCode = value;
                OnPropertyChanged("ErrorCode");
            }
        }
    }
}
