
namespace FenomPlus.ViewModels
{
    public class ChooseAgeViewModel : BaseViewModel
    {
        public ChooseAgeViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            //Services.BleHub.IsConnected();

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
    }
}
