﻿using System;
using System.Collections.Generic;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class BreathManeuverFeedbackView : BaseContentPage
    {
        private BreathManeuverFeedbackViewModel model;

        public BreathManeuverFeedbackView()
        {
            InitializeComponent();
            BindingContext = model = new BreathManeuverFeedbackViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
            App.BleDevice?.DisconnectAsync();
            Shell.Current.Navigation.PopToRootAsync();
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
    }
}
