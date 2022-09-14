using System;
using System.Collections.Generic;
using FenomPlus.Models;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class TestErrorView : BaseContentPage
    {
        private TestErrorViewModel model;

        public TestErrorView()
        {
            InitializeComponent();
            BindingContext = model = new TestErrorViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoToTutorial(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TutorialView)}?source=TestErrorView"), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancel(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(ChooseTestView)}"), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartTest(object sender, EventArgs e)
        {
            if (Cache.TestType == TestTypeEnum.Standard)
            {
                await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(StartTestView)}?test=Standard"), false);
            }
            else
            {
                await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(StartTestView)}?test=short"), false);
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
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }
    }
}
