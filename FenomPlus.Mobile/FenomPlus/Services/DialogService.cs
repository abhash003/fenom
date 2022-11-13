using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FenomPlus.Interfaces;

namespace FenomPlus.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public void ShowToast(string message, int seconds)
        {
            TimeSpan timeSpan = new TimeSpan(0,0, seconds);
            UserDialogs.Instance.Toast(message, timeSpan);
        }
    }
}
