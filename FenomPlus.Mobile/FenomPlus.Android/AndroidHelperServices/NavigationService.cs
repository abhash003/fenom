﻿using System;
using System.Threading.Tasks;
using FenomPlus.Controls;
using FenomPlus.Interfaces;
using FenomPlus.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Intent = Android.Content.Intent;
using AndroidApplication = Android.App.Application;

namespace FenomPlus.Services
{
    public partial class NavigationService : BaseService, INavigationService
    {
        public NavigationService(IAppServices services) : base(services)
        {
        }

        public void GotoBluetoothSettings()
        {
            using (Intent bluetoothPicker = new Intent("android.bluetooth.devicepicker.action.LAUNCH"))
            {
                bluetoothPicker.SetFlags(Android.Content.ActivityFlags.NewTask);
                AndroidApplication.Context.StartActivity(bluetoothPicker);
            }                
        }

        public async Task BreathManeuverFeedbackView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(BreathManeuverFeedbackView)}"), false);
        }

        public async Task DashboardView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(DashboardView)}"), false);
        }

        public async Task DeviceStatusHubView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(DeviceStatusHubView)}"), false);
        }

        public async Task DevicePowerOnView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(DevicePowerOnView)}"), false);
        }

        public async Task DeviceReadyView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(DeviceReadyView)}"), false);
        }

        public async Task HumanControlDisqualifiedView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(HumanControlDisqualifiedView)}"), false);
        }

        public async Task HumanControlPassedView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(HumanControlPassedView)}"), false);
        }

        public async Task HumanControlPreparingView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(HumanControlPreparingView)}"), false);
        }

        public async Task HumanControlPerformingView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(HumanControlPerformingView)}"), false);
        }

        public async Task NegativeControlFailView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(NegativeControlFailView)}"), false);
        }

        public async Task NegativeControlPassView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(NegativeControlPassView)}"), false);
        }

        public async Task NegativeControlPerformView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(NegativeControlPerformView)}"), false);
        }

        public async Task PairingView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(PairingView)}"), false);
        }

        public async Task PreparingStandardTestResultView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(PreparingStandardTestResultView)}"), false);
        }

        public async Task QualityControlView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(QualityControlView)}"), false);
        }

        public async Task QualityControlUsersView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(QualityControlUsersView)}"), false);
        }

        public async Task QualityControlDevicesView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(QualityControlDevicesView)}"), false);
        }

        public async Task StatusDeviceInfoView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(StatusDeviceInfoView)}"), false);
        }

        public async Task StopExhalingView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(StopExhalingView)}"), false);
        }

        public async Task TestErrorView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TestErrorView)}"), false);
        }

        public async Task TestFailedView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TestFailedView)}"), false);
        }

        public async Task TestResultsView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TestResultsView)}"), false);
        }

        public async Task TutorialView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(TutorialView)}"), false);
        }

        public async Task ViewPastResultsView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(ViewPastResultsView)}"), false);
        }

        public async Task ViewRecentErrorsView()
        {
            await Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(ViewRecentErrorsView)}"), false);
        }
        public void DisplayAlert(string title, string message, string cancel)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert(title, message, cancel);
            });
        }

        public async Task ShowStatusDetailsPopup(StatusButtonViewModel viewModel)
        {
            await Shell.Current.ShowPopupAsync(new StatusDetailsPopup(viewModel));
        }
    }
}