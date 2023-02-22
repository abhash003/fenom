using FenomPlus.ViewModels;
using FenomPlus.ViewModels.QualityControl.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FenomPlus.Controls
{
    public partial class QcButtonViewModel : INotifyPropertyChanged
    {
        private QCUser _qcUserModel;
        public QCUser QCUserModel
        {
            get => _qcUserModel;
            set
            {
                _qcUserModel = value;

                Assigned = _qcUserModel != null;
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(CurrentStatus));
                OnPropertyChanged(nameof(ExpiresDate));
                OnPropertyChanged(nameof(ExpiresDateString));
                OnPropertyChanged(nameof(NextTestDate));
                OnPropertyChanged(nameof(NextTestDateString));
                OnPropertyChanged(nameof(ShowChartOption));
            }
        }

        public bool Assigned { get; set; }

        public string UserName
        {
            get => QCUserModel.UserName;
            set
            {
                QCUserModel.UserName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string CurrentStatus
        {
            get => QCUserModel.CurrentStatus;
            set
            {
                QCUserModel.CurrentStatus = value;
                OnPropertyChanged(nameof(CurrentStatus));
            }
        }

        public DateTime ExpiresDate
        {
            get => QCUserModel.ExpiresDate;
            set
            {
                QCUserModel.ExpiresDate = value;
                OnPropertyChanged(nameof(ExpiresDate));
                OnPropertyChanged(nameof(ExpiresDateString));
            }
        }

        public string ExpiresDateString => QCUserModel.ExpiresDate != DateTime.MinValue ? QCUserModel.ExpiresDate.ToString("g", CultureInfo.CurrentCulture) : string.Empty;

        public DateTime NextTestDate
        {
            get => QCUserModel.NextTestDate;
            set
            {
                QCUserModel.NextTestDate = value;
                OnPropertyChanged(nameof(NextTestDate));
                OnPropertyChanged(nameof(NextTestDateString));
            }
        }

        public string NextTestDateString => QCUserModel.NextTestDate != DateTime.MinValue ? QCUserModel.NextTestDate.ToString("g", CultureInfo.CurrentCulture) : string.Empty;

        public bool ShowChartOption => QCUserModel.CurrentStatus == QCUser.UserQualified;

        public RelayCommand OpenChartCommand;

        public QcButtonViewModel()
        {
            OpenChartCommand = new RelayCommand(OpenChart);
        }

        private void OpenChart()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
