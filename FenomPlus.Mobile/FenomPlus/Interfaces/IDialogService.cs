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

        void ShowToast(string message, int seconds);
    }
}
