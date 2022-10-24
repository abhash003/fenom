using System;
using System.Threading.Tasks;

namespace FenomPlus.Interfaces
{
    public interface INavigationService
    {
        Task GotoBluetoothSettings();
        Task BreathManeuverFeedbackView();
        Task ChooseTestView();
        Task DevicePowerOnView();
        Task DeviceReadyView();
        Task HumanControlDisqualifiedView();
        Task HumanControlPassedView();
        Task HumanControlPreparingView();
        Task HumanControlPerformingView();
        Task NegativeControlFailView();
        Task NegativeControlPassView();
        Task NegativeControlPerformView();
        Task PreparingStandardTestResultView();
        Task QualityControlView();
        Task StopExhalingView();
        Task TestErrorView();
        Task TestFailedView();
        Task TestResultsView();
        Task TutorialView();
    }
}

