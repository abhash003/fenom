﻿using FenomPlus.Interfaces;
using FenomPlus.SDK.Abstractions;
using FenomPlus.SDK.Core.Ble.Interface;
using FenomPlus.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FenomPlus.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IBleDevice BleDevice => App.BleDevice;
        public IFenomHubSystemDiscovery FenomHub = App.FenomHubSystemDiscovery;
        public IAppServices Services => IOC.Services;

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        virtual public void OnAppearing()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public void OnDisappearing()
        {
        }
    }
}
