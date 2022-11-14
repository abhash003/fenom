using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FenomPlus.Interfaces;
using FenomPlus.SDK.Core.Features;
using Xamarin.Forms;

namespace FenomPlus.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public async Task ShowSecondsProgress(string message, int seconds)
        {
            IProgressDialog secondsProgressDialog = UserDialogs.Instance.Progress(message, null, null, true, MaskType.Gradient);

            double increment = Convert.ToDouble(100 / seconds);

            for (int i = 0; i < 100; i++)
            {
                secondsProgressDialog.PercentComplete = Convert.ToInt32(i * increment);

                if (secondsProgressDialog.PercentComplete >= 99)
                {
                    secondsProgressDialog.Dispose();
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task ShowLoadingAsync(string message, int seconds)
        {
            using (UserDialogs.Instance.Loading(message, null, null, true, MaskType.Black))
            {
                await Task.Delay(seconds * 1000);
            }
        }

        public void ShowToast(string message, int seconds)
        {
            TimeSpan timeSpan = new TimeSpan(0,0, seconds);
            UserDialogs.Instance.Toast(message, timeSpan);


            //ToastConfig toastConfig = new ToastConfig("Toast");
            //toastConfig.SetDuration(seconds * 1000);
            //toastConfig.SetBackgroundColor(Color.DimGray);
            //UserDialogs.Instance.Toast(toastConfig);
        }

        public async Task DatePromptAsync(string message, DateTime defaultDateTime)
        {
            await UserDialogs.Instance.DatePromptAsync(message, defaultDateTime);
        }
    }
}
