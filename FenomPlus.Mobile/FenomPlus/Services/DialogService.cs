
using Acr.UserDialogs;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Interfaces;
using Xamarin.Forms;

namespace FenomPlus.Services
{
    public class DialogService : IDialogService
    {
        public void ShowAlert(string message, string title, string buttonLabel)
        {
           UserDialogs.Instance.Alert(message, title, buttonLabel);
        }

        private IAsyncRelayCommand NextCommand;

        public async Task NotifyDevicePurgingAsync(int secondsRemaining, IAsyncRelayCommand nextCommand)
        {
            NextCommand = nextCommand;
            double increment = Convert.ToDouble(100 / secondsRemaining);

            using (var dlg = UserDialogs.Instance.Progress("Device purging..", CancelAction, "Cancel", true, MaskType.Black))
            {
                for (var i = 0; i < 99; i++)
                {
                    dlg.PercentComplete = Convert.ToInt32(i * increment);

                    if (dlg.PercentComplete <= 99)
                    {
                        await Task.Delay(1000);
                    }
                    else
                    {
                        NextCommand?.Execute(null);
                        break;
                    }
                }
            }
        }

        private void CancelAction()
        {
            // Nothing here - but required stub to show Cancel button.
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
