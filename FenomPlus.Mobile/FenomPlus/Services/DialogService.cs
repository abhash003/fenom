
using Acr.UserDialogs;
using System;
using System.Threading.Tasks;
using FenomPlus.Interfaces;

namespace FenomPlus.Services
{
    public class DialogService : IDialogService
    {
        //public Task ShowAlertAsync(string message, string title, string buttonLabel)
        //{
        //    return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        //}

        public void ShowAlert(string message, string title, string buttonLabel)
        {
           UserDialogs.Instance.Alert(message, title, buttonLabel);
        }

        private IProgressDialog SecondsProgressDialog;

        public async Task ShowSecondsProgressAsync(string message, int seconds)
        {
            SecondsProgressDialog = UserDialogs.Instance.Progress(message, null, null, true, MaskType.None);

            double increment = Convert.ToDouble(100 / seconds);

            for (int i = 0; i < 100; i++)
            {
                SecondsProgressDialog.PercentComplete = Convert.ToInt32(i * increment);

                if (SecondsProgressDialog.PercentComplete >= 99)
                {
                    SecondsProgressDialog.Dispose();
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        public void ShowSecondsProgress(string message, int seconds)
        {
            SecondsProgressDialog = UserDialogs.Instance.Progress(message, null, null, true, MaskType.None);

            double increment = Convert.ToDouble(100 / seconds);

            for (int i = 0; i < 100; i++)
            {
                SecondsProgressDialog.PercentComplete = Convert.ToInt32(i * increment);

                if (SecondsProgressDialog.PercentComplete >= 99)
                {
                    SecondsProgressDialog.Dispose();
                }
            }
        }

        public bool SecondsProgressDialogShowing()
        {
            if (SecondsProgressDialog != null)
                return SecondsProgressDialog.IsShowing;
            else
                return false;
        }

        public void DismissSecondsProgressDialog()
        {
            SecondsProgressDialog.Dispose();
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
            //TimeSpan timeSpan = new TimeSpan(0,0, seconds);
            //UserDialogs.Instance.Toast(message, timeSpan);


            ToastConfig toastConfig = new ToastConfig("Toast");
            toastConfig.SetDuration(seconds * 1000);
            toastConfig.SetBackgroundColor(Xamarin.Forms.Color.Transparent);
            UserDialogs.Instance.Toast(toastConfig);
        }

        public async Task DatePromptAsync(string message, DateTime defaultDateTime)
        {
            await UserDialogs.Instance.DatePromptAsync(message, defaultDateTime);
        }
    }
}
