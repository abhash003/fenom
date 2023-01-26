using Acr.UserDialogs;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FenomPlus.Interfaces
{
    public interface IDialogService
    {
        //Task ShowAlertAsync(string message, string title, string buttonLabel);
        public void ShowAlert(string message, string title, string buttonLabel);

        Task NotifyDevicePurgingAsync(int secondsRemaining, IAsyncRelayCommand nextCommand);

        //bool SecondsProgressDialogShowing();

        //void DismissSecondsProgressDialog();

        Task ShowLoadingAsync(string message, int seconds);

        void ShowToast(string message, int seconds);

        Task DatePromptAsync(string message, DateTime defaultDateTime);

    }
}
