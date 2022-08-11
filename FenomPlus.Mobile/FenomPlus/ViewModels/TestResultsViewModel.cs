using FenomPlus.Models;

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
        override public void OnAppearing()
        {
            base.OnAppearing();
            if (Cache.TestType == TestTypeEnum.Standard)
            {
                TestType = "Standard";
            }
            else
            {
                TestType = "Short";
            }

            TestResult = (Cache.NOScore) < 5 ? "< 5" :
                        (Cache.NOScore) > 300 ? "> 300":
                        Cache.NOScore.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnDisappearing()
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
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
