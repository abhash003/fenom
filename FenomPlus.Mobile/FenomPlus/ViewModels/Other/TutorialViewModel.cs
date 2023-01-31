using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.Helpers;
using FenomPlus.SDK.Core.Models;
using FenomPlus.Services.DeviceService.Enums;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FenomPlus.ViewModels
{
    public partial class TutorialViewModel : BaseViewModel
    {
        [ObservableProperty]
        private int _tutorialIndex = 1;

        [ObservableProperty]
        private bool _instructionsVisible;

        [ObservableProperty]
        private bool _illustrationVisible;

        [ObservableProperty]
        private bool _breathGaugeVisible;

        [ObservableProperty]
        private bool _successPanelVisible;

        [ObservableProperty] 
        private string _stepTitle;

        [ObservableProperty] 
        private string _instructionsText;

        [ObservableProperty] 
        private string _illustrationSource;

        [ObservableProperty]
        private bool _showGauge;

        [ObservableProperty]
        private float _gaugeData;
        partial void OnGaugeDataChanged(float value)
        {
            if (TutorialIndex == 5)
            {
                PlaySounds.PlaySound(value);
            }
            else
            {
                PlaySounds.StopAll();
            }
        }

        [ObservableProperty]
        private string _gaugeStatus;

        [ObservableProperty]
        protected bool _showBack;

        [ObservableProperty]
        protected bool _showNext;

        [ObservableProperty]
        private string _testType;

        [ObservableProperty]
        private int _testTime;

        [ObservableProperty]
        protected bool _showTutorial = true;

        [ObservableProperty]
        protected bool _showSuccess;


        public TutorialViewModel()
        {
        }

        private void UpdateContent()
        {
            switch (TutorialIndex)
            {
                // Pages
                case 1:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGaugeVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = false;
                    ShowNext = true;

                    StepTitle = "Step 1";
                    IllustrationSource = "TutStep1";
                    InstructionsText = "Snap new mouthpiece onto the device";
                    break;

                case 2:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGaugeVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 2";
                    IllustrationSource = "TutStep2";
                    InstructionsText = "Firmly grasp the device";
                    break;

                case 3:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGaugeVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 3";
                    IllustrationSource = "TutStep3";
                    InstructionsText = "Sit up straight";
                    break;

                case 4:
                    InstructionsVisible = true;
                    IllustrationVisible = true;
                    BreathGaugeVisible = false;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 4";
                    IllustrationSource = "TutStep4";
                    InstructionsText = "Take a deep breath\n\nPlace your lips around the mouthpiece";
                    break;

                case 5: // Breath Gauge
                    InstructionsVisible = true;
                    IllustrationVisible = false;
                    BreathGaugeVisible = true;
                    SuccessPanelVisible = false;

                    ShowBack = true;
                    ShowNext = true;

                    StepTitle = "Step 5";
                    IllustrationSource = "TutStep5";
                    InstructionsText = "Exhale into the device now\n\nPractice pointing the needle at the star";
                    break;

                case 6: // success Page
                    InstructionsVisible = false;
                    IllustrationVisible = false;
                    BreathGaugeVisible = false;
                    SuccessPanelVisible = true;

                    ShowBack = true;
                    ShowNext = false;

                    StepTitle = string.Empty;
                    IllustrationSource = string.Empty;
                    InstructionsText = string.Empty;
                    break;
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();

            if (Services.DeviceService.Current != null)
            {
                GaugeData = Services.DeviceService.Current.BreathFlow = 0;
                GaugeStatus = "Start Blowing";

                // Monitor event
                Services.DeviceService.Current.BreathFlowChanged += CacheOnBreathFlowChanged;
            }

            // Reset to first page
            TutorialIndex = 1;
            UpdateContent();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            PlaySounds.StopAll();

            if (Services.DeviceService.Current != null)
            {
                Services.DeviceService.Current.BreathFlowChanged -= CacheOnBreathFlowChanged;

                if (Services.DeviceService.Current?.BreathTestInProgress == true)
                {
                    _ = Services.DeviceService.Current.StartTest(BreathTestEnum.Stop);
                }
            }
        }

        private void CacheOnBreathFlowChanged(object sender, EventArgs e)
        {
            if (Services.DeviceService.Current != null) GaugeData = Services.DeviceService.Current.BreathFlow;

            if (GaugeData < Config.GaugeDataLow)
            {
                GaugeStatus = "Exhale Harder";
            }
            else if (GaugeData > Config.GaugeDataHigh)
            {
                GaugeStatus = "Exhale Softer";
            }
            else
            {
                GaugeStatus = "Good Job!";
            }
        }

        public void NewGlobalData()
        {
            if (Services.DeviceService.Current != null) 
                GaugeData = Services.DeviceService.Current.BreathFlow;
        }

        [RelayCommand]
        private async Task Next()
        {
            int requestedIndex = TutorialIndex + 1;

            if (requestedIndex > 6)
                requestedIndex = 6;

            if (requestedIndex == 5) // Page with Breath Gauge
            {
                if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
                {
                    DeviceCheckEnum deviceStatus = Services.DeviceService.Current.CheckDeviceBeforeTest();

                    switch (deviceStatus)
                    {
                        case DeviceCheckEnum.Ready:
                            await Services.DeviceService.Current.StartTest(BreathTestEnum.Training);
                            break;
                        
                        case DeviceCheckEnum.DevicePurging:
                            await Services.Dialogs.NotifyDevicePurgingAsync(Services.DeviceService.Current.DeviceReadyCountDown);
                            if (Services.Dialogs.PurgeCancelRequest)
                            {
                                UpdateContent(); // Force a faster refresh
                                return; // Don't Increment
                            }
                            break;

                        case DeviceCheckEnum.HumidityOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run test. Humidity level ({Services.DeviceService.Current.EnvironmentalInfo.Humidity}%) is out of range.", "Humidity Warning", "Close");
                            return; // Don't Increment
                        
                        case DeviceCheckEnum.PressureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run test. Pressure level ({Services.DeviceService.Current.EnvironmentalInfo.Pressure} kPa) is out of range.", "Pressure Warning", "Close");
                            return; // Don't Increment
                        
                        case DeviceCheckEnum.TemperatureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run test. Temperature level ({Services.DeviceService.Current.EnvironmentalInfo.Temperature} °C) is out of range.", "Temperature Warning", "Close");
                            return; // Don't Increment
                        
                        case DeviceCheckEnum.BatteryCriticallyLow:
                            Services.Dialogs.ShowAlert($"Unable to run test. Battery Level ({Services.DeviceService.Current.EnvironmentalInfo.BatteryLevel}%) is critically low: ", "Battery Warning", "Close");
                            return; // Don't Increment

                        case DeviceCheckEnum.NoSensorMissing:
                            Services.Dialogs.ShowAlert($"NO Sensor is missing.  Install a F150 sensor.", "Sensor Error", "Close");
                            return; // Don't Increment

                        case DeviceCheckEnum.NoSensorCommunicationFailed:
                            Services.Dialogs.ShowAlert($"NO Sensor communication failed.", "Sensor Error", "Close");
                            return; // Don't Increment

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else // Not asking for breath test page
            {
                if (Services.DeviceService.Current?.BreathTestInProgress == true)
                {
                    await Services.DeviceService.Current.StartTest(BreathTestEnum.Stop);
                }
            }

            TutorialIndex = requestedIndex;
            UpdateContent();

            Debug.WriteLine($"***** Current Index = {TutorialIndex}");
        }


        [RelayCommand]
        private async Task Back()
        {
            int requestedIndex = TutorialIndex - 1;

            if (requestedIndex < 1)
                requestedIndex = 1;

            if (requestedIndex == 5) // Page with Breath Gauge
            {
                if (Services.DeviceService.Current != null && Services.DeviceService.Current.IsNotConnectedRedirect())
                {
                    DeviceCheckEnum deviceStatus = Services.DeviceService.Current.CheckDeviceBeforeTest();

                    switch (deviceStatus)
                    {
                        case DeviceCheckEnum.Ready:
                            await Services.DeviceService.Current.StartTest(BreathTestEnum.Training);
                            break;
                        
                        case DeviceCheckEnum.DevicePurging:
                            await Services.Dialogs.NotifyDevicePurgingAsync(Services.DeviceService.Current.DeviceReadyCountDown);
                            if (Services.Dialogs.PurgeCancelRequest)
                            {
                                return;
                            }
                            break;
                        
                        case DeviceCheckEnum.HumidityOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Humidity level ({Services.DeviceService.Current.EnvironmentalInfo.Humidity}%) is out of range.", "Humidity Warning", "Close");
                            return; // Don't Increment
                        
                        case DeviceCheckEnum.PressureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Pressure level ({Services.DeviceService.Current.EnvironmentalInfo.Pressure} kPa) is out of range.", "Pressure Warning", "Close");
                            return; // Don't Increment
                        
                        case DeviceCheckEnum.TemperatureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Temperature level ({Services.DeviceService.Current.EnvironmentalInfo.Temperature} °C) is out of range.", "Temperature Warning", "Close");
                            return; // Don't Increment
                        
                        case DeviceCheckEnum.BatteryCriticallyLow:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Battery Level ({Services.DeviceService.Current.EnvironmentalInfo.BatteryLevel}%) is critically low: ", "Battery Warning", "Close");
                            return; // Don't Increment

                        case DeviceCheckEnum.NoSensorMissing:
                            Services.Dialogs.ShowAlert($"NO Sensor is missing.  Install a F150 sensor.", "Sensor Error", "Close");
                            return; // Don't Increment

                        case DeviceCheckEnum.NoSensorCommunicationFailed:
                            Services.Dialogs.ShowAlert($"NO Sensor communication failed.", "Sensor Error", "Close");
                            return; // Don't Increment

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else // Not asking for breath test page
            {
                if (Services.DeviceService.Current?.BreathTestInProgress == true)
                {
                    await Services.DeviceService.Current.StartTest(BreathTestEnum.Stop);
                }
            }

            TutorialIndex = requestedIndex;
            UpdateContent();

            Debug.WriteLine($"***** Current Index = {TutorialIndex}");
        }
    }
}