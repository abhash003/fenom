
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

        private double _CalibrationValue1;
        public double CalibrationValue1
        {
            get => _CalibrationValue1;
            set
            {
                _CalibrationValue1 = value;
                OnPropertyChanged("CalibrationValue1");
            }
        }

        private double _CalibrationValue2;
        public double CalibrationValue2
        {
            get => _CalibrationValue2;
            set
            {
                _CalibrationValue2 = value;
                OnPropertyChanged("CalibrationValue2");
            }
        }

        private double _CalibrationValue3;
        public double CalibrationValue3
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
