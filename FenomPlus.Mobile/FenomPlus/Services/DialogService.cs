
using Acr.UserDialogs;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FenomPlus.Interfaces;
using Xamarin.Forms;

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
        private int ProgressValue = 0;

        public async Task ShowSecondsProgressAsync(string message, int seconds)
        {
            if (SecondsProgressDialog is { IsShowing: true })
                return;

            //SecondsProgressDialog = UserDialogs.Instance.Progress(message, null, null, true, MaskType.None);

            double increment = Convert.ToDouble(100 / seconds);

            //for (int i = 0; i < 100; i++)
            //{
            //    SecondsProgressDialog.PercentComplete = Convert.ToInt32(i * increment);

            //    if (SecondsProgressDialog.PercentComplete >= 99)
            //    {
            //        DismissSecondsProgressDialog();
            //    }
            //    else
            //    {
            //        await Task.Delay(1000);
            //    }
            //}

            ProgressValue = 0;

            Device.BeginInvokeOnMainThread(async () =>
            {
                using (SecondsProgressDialog = UserDialogs.Instance.Progress(message, null, null, true, MaskType.None))
                {
                    await Task.Run(async () =>
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            int newProgress = Convert.ToInt32(i * increment);
                            if (newProgress > ProgressValue)
                            {
                                ProgressValue = newProgress;
                            }

                            if (ProgressValue > SecondsProgressDialog.PercentComplete)
                            {
                                SecondsProgressDialog.PercentComplete = ProgressValue;

                                if (SecondsProgressDialog.PercentComplete >= 99)
                                {
                                    DismissSecondsProgressDialog();
                                }
                                else
                                {
                                    await Task.Delay(1000);
                                }
                            }


                        }
                    });
                }
            });
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
            if (SecondsProgressDialog != null)
            {
                SecondsProgressDialog.Hide();
                SecondsProgressDialog.Dispose();
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
