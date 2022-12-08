using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Enums;

namespace FenomPlus.ViewModels
{
    public partial class TestResultsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _testType;

        [ObservableProperty]
        private string _testResult;

        public TestResultsViewModel()
        {
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            if (Cache.TestType == TestTypeEnum.Standard)
            {
                TestType = "10-second";
            }
            else
            {
                TestType = "6-second";
            }

            TestResult = (Cache.FenomValue) < 5 ? "< 5" :
                        (Cache.FenomValue) > 300 ? "> 300":
                        Cache.FenomValue.ToString(CultureInfo.InvariantCulture);

            BleHub.ReadyForTest = false;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
