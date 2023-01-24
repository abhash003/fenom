using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace FenomPlus.ViewModels
{
    public partial class QCNegativeControlViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _deviceSerialNumber = string.Empty;

        [ObservableProperty]
        private string _deviceStatus = string.Empty;

        [ObservableProperty]
        private string _lastTestStatus = string.Empty;

        [ObservableProperty]
        private DateTime _lastTestDate = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _expiresDate = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _nextTestDate = DateTime.MinValue;

        [ObservableProperty] 
        private List<double> _chartData = new List<double>();

        public QCNegativeControlViewModel()
        {
        }
    }
}
