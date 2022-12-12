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

            TestResult = (Services.Cache.FenomValue) < 5 ? "< 5" :
                        (Services.Cache.FenomValue) > 300 ? "> 300":
                        Services.Cache.FenomValue.ToString(CultureInfo.InvariantCulture);

            Services.Device.ReadyForTest = false;
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

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
