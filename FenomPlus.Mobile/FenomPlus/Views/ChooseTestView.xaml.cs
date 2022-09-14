using System;
using System.Threading.Tasks;
using FenomPlus.ViewModels;
using Xamarin.Forms;
using FenomPlus.Models;

namespace FenomPlus.Views
{
    public partial class ChooseTestView : BaseContentPage
    {
        private ChooseTestViewModel model;
        private bool isBackdropTapEnabled;
        private double offsetY = 10;
        private uint duration = 100;

        public ChooseTestView()
        {
            InitializeComponent();
            BindingContext = model = new ChooseTestViewModel();
            bool scanning = Services.BleHub.IsScanning;
            //Shell.Current.IsVisible = false;
        }

        private async Task OpenDrawer()
        {
            await Task.WhenAll(
                Backdrop.FadeTo(1, length: duration),
                Drawer.TranslateTo(0, offsetY, duration, Easing.SinIn)
            );
            isBackdropTapEnabled = true;
            Backdrop.InputTransparent = false;
        }

        private async Task CloseDrawer()
        {
            await Task.WhenAll(
                Backdrop.FadeTo(0, length: duration),
                Drawer.TranslateTo(0, 200, duration, Easing.SinIn)
            );
            isBackdropTapEnabled = false;
            Backdrop.InputTransparent = true;
        }

        private async void OnBackdropTapped(object sender, EventArgs e)
        {
            if (isBackdropTapEnabled)
            {
                await CloseDrawer();
            }
        }

        private async void OnSwipeUp(object sender, SwipedEventArgs e)
        {
            await OpenDrawer();
        }

        private async void OnSwipeDown(object sender, SwipedEventArgs e)
        {
            await CloseDrawer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnStandartTest(object sender, EventArgs e)
        {
            if (Services.BleHub.IsConnected() && Cache.ReadyForTest)
            {
                Cache.TestType = TestTypeEnum.Standard;
                await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(BreathManeuverFeedbackView)}"), false);
//                await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(StartTestView)}?test=Standard"), false);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShortTest(object sender, EventArgs e)
        {
            if (Services.BleHub.IsConnected() && Cache.ReadyForTest)
            {
                Cache.TestType = TestTypeEnum.Short;
                await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(BreathManeuverFeedbackView)}"), false);
//                await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(StartTestView)}?test=short"), false);
            }
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Error_Clicked(System.Object sender, System.EventArgs e)
        {

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
