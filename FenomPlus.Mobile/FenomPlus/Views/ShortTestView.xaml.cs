using System;
using System.Collections.Generic;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class ShortTestView : BaseContentPage
    {
        private ShortTestViewModel model;

        public ShortTestView()
        {
            InitializeComponent();
            BindingContext = model = new ShortTestViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoToTutorial(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TutorialView)}?source=ShortTestView"), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancel(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(new ShellNavigationState(".."), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartShortTest(object sender, EventArgs e)
        {
            // start test here
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
