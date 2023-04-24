using System.Globalization;
using FenomPlus.Enums;

namespace FenomPlus.ViewModels
{
    public class TestResultsViewModel : BaseViewModel
    {
        public TestResultsViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing();
            if (Services.Cache.TestType == TestTypeEnum.Standard)
            {
                TestType = "10-second";
            }
            else
            {
                TestType = "6-second";
            }

            if (Services.DeviceService.Current == null) return;

            string tmp = Services.DeviceService.Current.FenomValue?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            TestResult = (Services.DeviceService.Current.FenomValue) < 5 ? "< 5" :
                        (Services.DeviceService.Current.FenomValue) > 300 ? "> 300" : tmp;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappearing()
        {
            Services.Cache.TestType = TestTypeEnum.None;
            base.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        private string _TestType;
        public string TestType
        {
            get => _TestType;
            set
            {
                _TestType = value;
                OnPropertyChanged("TestType");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string testResult;
        public string TestResult
        {
            get => testResult;
            set
            {
                testResult = value;
                OnPropertyChanged("TestResult");
            }
        }

    }
}
