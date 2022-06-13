﻿using FenomPlus.Database.Repository.Interfaces;
using FenomPlus.Interfaces;
using FenomPlus.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FenomPlus.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, IBaseServices
    {
        public IAppServices Services => IOC.Services;
        public IBleHubService BleHub => Services.BleHub;
        public ICacheService Cache => Services.Cache;
        
        // repos here
        public IBreathManeuverErrorRepository ErrorsRepo => Services.Database.BreathManeuverErrorRepo;
        public IBreathManeuverResultRepository ResultsRepo => Services.Database.BreathManeuverResultRepo;
        public IQualityControlRepository QCRepo => Services.Database.QualityControlRepo;
        public IQualityControlDevicesRepository QCDevicesRepo => Services.Database.QualityControlDevicesRepo;
        public IQualityControlUsersRepository QCUsersRepo => Services.Database.QualityControlUsersRepo;

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string deviceSerialNumber;
        public string DeviceSerialNumber
        {
            get { return deviceSerialNumber; }
            set { SetProperty(ref deviceSerialNumber, value);  }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <param name="onChanged"></param>
        /// <returns></returns>
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
        public BaseViewModel()
        {
            DeviceSerialNumber = Services.Cache.DeviceSerialNumber;
        }

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

        /// <summary>
        /// 
        /// </summary>
        virtual public void NewGlobalData()
        {
        }
    }
}
