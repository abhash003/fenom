﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FenomPlus.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FenomPlus.Models;

namespace FenomPlus.Views
{
    public partial class ChooseTestView : BaseContentPage
    {
        private ChooseTestViewModel model;
        private bool isBackdropTapEnabled;
        private double offsetY = 20;
        private uint duration = 100;

        public ChooseTestView()
        {
            InitializeComponent();
            BindingContext = model = new ChooseTestViewModel();
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
            App.TestType = TestTypeEnum.Standard;
            await Shell.Current.GoToAsync(new ShellNavigationState($"///StartTestView?test=Standard"), false);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnShortTest(object sender, EventArgs e)
        {
            App.TestType = TestTypeEnum.Short;
            await Shell.Current.GoToAsync(new ShellNavigationState($"///StartTestView?test=short"), false);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
            if ((App.BleDevice == null) || (App.BleDevice.Connected == false))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //await Shell.Current.GoToAsync(new ShellNavigationState("DevicePowerOnView"), false);
                });
            }
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
    }
}
