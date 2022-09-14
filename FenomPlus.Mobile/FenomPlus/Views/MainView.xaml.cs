using System;
using System.Collections.Generic;
using FenomPlus.Services;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class MainView : Shell
    {
        private MainViewModel model;

        public MainView()
        {
            InitializeComponent();
            BindingContext = model = new MainViewModel();
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
        public void NotifyViews()
        {
            if ((Current == null) || (Current.CurrentPage == null)) return;
            try
            {
                Page page = Current.CurrentPage;
                ((BaseContentPage)page).NewGlobalData();
            } catch(Exception ex) {
                IOC.Services.LogCat.Print(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void NotifyViewModels()
        {

        }
    }
}
