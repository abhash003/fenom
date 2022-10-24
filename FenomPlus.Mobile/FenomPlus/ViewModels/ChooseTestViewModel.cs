﻿using System;
using System.Windows.Input;
using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using Xamarin.Forms;

namespace FenomPlus.ViewModels
{
    public class ChooseTestViewModel : BaseViewModel
    {

        public ChooseTestViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        override public void OnAppearing()
        {
            base.OnAppearing();
            Services.BleHub.IsConnected();
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
