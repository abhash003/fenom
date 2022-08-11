
namespace FenomPlus.ViewModels
{
    public class CalibrationViewModel : BaseViewModel
    {
        public CalibrationViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
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
        override public void NewGlobalData()
        {
            base.NewGlobalData();
        }

        private string _CalibrationValue1;
        public string CalibrationValue1
        {
            get => _CalibrationValue1;
            set
            {
                _CalibrationValue1 = value;
                OnPropertyChanged("CalibrationValue1");
            }
        }

        private string _CalibrationValue2;
        public string CalibrationValue2
        {
            get => _CalibrationValue2;
            set
            {
                _CalibrationValue2 = value;
                OnPropertyChanged("CalibrationValue2");
            }
        }

        private string _CalibrationValue3;
        public string CalibrationValue3
        {
            get => _CalibrationValue3;
            set
            {
                _CalibrationValue3 = value;
                OnPropertyChanged("CalibrationValue3");
            }
        }
    }
}
