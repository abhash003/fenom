using FenomPlus.ViewModels;
using FenomPlus.ViewModels.QualityControl.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xamarin.Forms;
using FenomPlus.Services;
using FenomPlus.Interfaces;
using TinyIoC;

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

        public bool Assigned { get; set; } = false;

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
                OnPropertyChanged(nameof(ShowChartOption));
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
                OnPropertyChanged(nameof(ExpiresVisible));
            }
        }

        public string ExpiresDateString => QCUserModel.ExpiresDate != DateTime.MinValue ? QCUserModel.ExpiresDate.ToString("g", CultureInfo.CurrentCulture) : string.Empty;


        public const string UserConditionallyQualified = "Conditionally Qualified";
        public const string UserQualified = "Qualified";
        public const string UserDisqualified = "Disqualified";
        public const string UserNone = "None";

        public bool ExpiresVisible => CurrentStatus == QCUser.UserConditionallyQualified;

        public DateTime NextTestDate
        {
            get => QCUserModel.NextTestDate;
            set
            {
                QCUserModel.NextTestDate = value;
                OnPropertyChanged(nameof(NextTestDate));
                OnPropertyChanged(nameof(NextTestVisible));
            }
        }

        public string NextTestDateString => QCUserModel.NextTestDate != DateTime.MinValue ? QCUserModel.NextTestDate.ToString("g", CultureInfo.CurrentCulture) : string.Empty;

        public bool NextTestVisible => CurrentStatus == QCUser.UserConditionallyQualified;

        public bool ShowChartOption => QCUserModel is { CurrentStatus: QCUser.UserQualified };

        public RelayCommand OpenChartCommand;

        public QcButtonViewModel()
        {
            // Don't assign through property when initializing because it is not being assigned a valid model yet
            _qcUserModel = new QCUser(string.Empty, string.Empty);
            OpenChartCommand = new RelayCommand(OpenChart);
        }

        private void OpenChart()
        {
            var navigation = TinyIoCContainer.Current.Resolve<INavigationService>();
            var vm = TinyIoCContainer.Current.Resolve<QualityControlViewModel>();
            navigation.ShowQCChartPopup(vm, QCUserModel);
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
