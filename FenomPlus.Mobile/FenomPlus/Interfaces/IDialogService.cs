using Acr.UserDialogs;
using FenomPlus.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FenomPlus.Interfaces
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);

        Task ShowSecondsProgress(string message, int seconds);

        bool SecondsProgressDialogShowing();

        void DismissSecondsProgressDialog()

        Task ShowLoadingAsync(string message, int seconds);

        void ShowToast(string message, int seconds);

        Task DatePromptAsync(string message, DateTime defaultDateTime);
    }
}
