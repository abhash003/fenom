using System;
using System.Threading.Tasks;
using FenomPlus.ViewModels;
using Xamarin.Forms;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;

namespace FenomPlus.Views
{
    public partial class ChooseTestView : BaseContentPage
    {
        private ChooseTestViewModel model;

        public ChooseTestView()
        {
            InitializeComponent();
            BindingContext = model = new ChooseTestViewModel();
            //bool scanning = Services.BleHub.IsScanning;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnStandartTest(object sender, EventArgs e)
        {
            if (Services.BleHub.IsNotConnectedRedirect() && Cache.ReadyForTest)
            {
                Cache.TestType = TestTypeEnum.Standard;
                await BleHub.StartTest(BreathTestEnum.Start10Second);
                await Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShortTest(object sender, EventArgs e)
        {
            if (Services.BleHub.IsNotConnectedRedirect() && Cache.ReadyForTest)
            {
                Cache.TestType = TestTypeEnum.Short;
                await BleHub.StartTest(BreathTestEnum.Start6Second);
                await Services.Navigation.BreathManeuverFeedbackView();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnTutorial(object sender, EventArgs e)
        {
            await Services.Navigation.TutorialView();
        }
        

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }
    }
}
