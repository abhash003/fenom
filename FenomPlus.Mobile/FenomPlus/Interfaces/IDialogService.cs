using Acr.UserDialogs;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FenomPlus.Interfaces
{
    public interface IDialogService
    {
        public void ShowAlert(string message, string title, string buttonLabel);

        // Indicates if a cancellation was requested in NotifyDevicePurgingAsync
        public bool PurgeCancelRequest { get; set; }

        Task NotifyDevicePurgingAsync(int secondsRemaining);

        Task ShowLoadingAsync(string message, int seconds);

        void ShowToast(string message, int seconds);

        Task DatePromptAsync(string message, DateTime defaultDateTime);

        Task<string> UserNamePromptAsync();

    }
}
