using FenomPlus.Controls;
using FenomPlus.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FenomPlus.Interfaces
{
    public interface INavigationService
    {
        void GotoBluetoothSettings();
        Task BreathManeuverFeedbackView();
        Task DashboardView();
        Task DeviceStatusHubView();
        Task DevicePowerOnView();
        Task DeviceReadyView();
        Task PreparingStandardTestResultView();

        Task QualityControlView();
        Task QCNegativeControlTestView();
        Task QCNegativeControlResult();
        Task QCNegativeControlChartView();
        Task QCUserTestView();

        Task QCUserStopTestView();
        Task QCUserTestCalculationView();
        Task QCUserTestResultView();
        Task QCUserTestErrorView();
        Task QCUserTestChartView();
        Task QCSettingsView();

        Task StopExhalingView();
        Task TestErrorView();
        Task TestFailedView();
        Task TestResultsView();
        Task TutorialView();

        void DisplayAlert(string title, string message, string cancel);
        Task ShowStatusDetailsPopup(StatusButtonViewModel viewModel);

    }
}

