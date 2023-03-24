
using Acr.UserDialogs;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Interfaces;
using Xamarin.Forms;
using System.Threading;

namespace FenomPlus.Services
{
    public class DialogService : IDialogService
    {
        public void ShowAlert(string message, string title, string buttonLabel)
        {
           UserDialogs.Instance.Alert(message, title, buttonLabel);
        }

        public async Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            await UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        //public async Task<string> UserNamePromptAsync()
        //{

        //    var result = await UserDialogs.Instance.PromptAsync("User Name", "Create new QC User", "Create", "Cancel", "", InputType.Name);

        //    if (!result.Ok)
        //        return "cancel";

        //    return result.Text;
        //}

        public async Task<bool> ShowConfirmYesNo(string message, string title)
        {
            //var result = UserDialogs.Instance.ConfirmAsync(message, title, "Yes", "No");

            var r = await UserDialogs.Instance.ConfirmAsync("message", title);
            return r;
        }

        public bool PurgeCancelRequest { get; set; }

        public async Task NotifyDevicePurgingAsync(int secondsRemaining)
        {
            double increment = Convert.ToDouble(100 / secondsRemaining);
            PurgeCancelRequest = false;

            using (var dlg = UserDialogs.Instance.Progress("Device purging..", PurgeCancelAction, "Cancel", true, MaskType.Black))
            {
                for (var i = 0; i < 99; i++)
                {
                    if (PurgeCancelRequest)
                    {
                        break;
                    }


                    dlg.PercentComplete = Convert.ToInt32(i * increment);

                    if (dlg.PercentComplete <= 99)
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }

        private void PurgeCancelAction()
        {
            PurgeCancelRequest = true;
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

        public async Task<string> UserNamePromptAsync()
        {

            var result = await UserDialogs.Instance.PromptAsync("User Name", "Create new QC User", "Create", "Cancel", "", InputType.Name);

            if (!result.Ok)
                return "cancel";

            return result.Text;
        }
    }
}
