using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using FenomPlus.Database.Adapters;
using FenomPlus.Database.Tables;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.ViewModels
{
    public partial class QCUserViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _deviceSerialNumber = string.Empty;

        [ObservableProperty]
        private string _currentStatus = string.Empty;

        [ObservableProperty] private DateTime _dateCreated = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _expiresDate = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _nextTestDate = DateTime.MinValue;;

        [ObservableProperty]
        private List<QCTestInfo> _testResults = new List<QCTestInfo>();

        public QCUserViewModel()
        {
        }
    }
}
