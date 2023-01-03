using FenomPlus.Helpers;
using FenomPlus.Models;
using FenomPlus.SDK.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Pdf.Graphics;
using Xamarin.Forms;
using FenomPlus.Enums;
using FenomPlus.Services;

namespace FenomPlus.ViewModels
{
    public partial class TutorialViewModel : BaseViewModel
    {
        [ObservableProperty]
        private int _tutorialIndex = 1;

        //partial void OnTutorialIndexChanged(int value)
        //{
        //    if (TutorialIndex < 1)
        //        TutorialIndex = 1;

        //    if (TutorialIndex > 6)
        //        TutorialIndex = 6;

        //    //int priorTutorialIndex = TutorialIndex;

        //    //if (TutorialIndex is 4 or 6)
        //    //{
        //    //    if (Services.DeviceService.Current != null)
        //    //    {
        //    //        Services.DeviceService.Current.StartTest(BreathTestEnum.Stop);
        //    //    }
        //    //    else
        //    //    {
        //    //        if (Services.DeviceService.Devices.Count > 0)
        //    //        {
        //    //            Services.DeviceService.Devices[0].ConnectAsync();
        //    //        }
        //    //        else
        //    //        {
        //    //            Console.WriteLine("####Devices not found####");
        //    //        }
        //    //    }
                
        //    //    PlaySounds.StopAll();
        //    //}

        //    //UpdateContent();
        //}

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

                case 5:
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

            GaugeData = Services.Cache.BreathFlow = 0;
            GaugeStatus = "Start Blowing";

            // Monitor event
            Services.Cache.BreathFlowChanged += CacheOnBreathFlowChanged;

            // Reset to first page
            TutorialIndex = 1;
            UpdateContent();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            PlaySounds.StopAll();

            Services.Cache.BreathFlowChanged -= CacheOnBreathFlowChanged;

            if (Services.DeviceService.Current?.BreathTestInProgress == true)
            {
                Services.DeviceService.Current.StartTest(BreathTestEnum.Stop);
            }

            if (Services.Dialogs.SecondsProgressDialogShowing())
            {
                Services.Dialogs.DismissSecondsProgressDialog();
            }
        }

        private void CacheOnBreathFlowChanged(object sender, EventArgs e)
        {
            GaugeData = Services.Cache.BreathFlow;

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

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            GaugeData = Services.Cache.BreathFlow;
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
                    CacheService.DeviceCheckEnum deviceStatus = Services.Cache.CheckDeviceBeforeTest();

                    switch (deviceStatus)
                    {
                        case CacheService.DeviceCheckEnum.Ready:
                            await Services.DeviceService.Current.StartTest(BreathTestEnum.Training);
                            break;
                        case CacheService.DeviceCheckEnum.DevicePurging:
                            await Services.Dialogs.ShowSecondsProgressAsync($"Device purging..", Services.DeviceService.Current.DeviceReadyCountDown);
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.HumidityOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run test. Humidity level ({Services.Cache.EnvironmentalInfo.Humidity}%) is out of range.", "Humidity Warning", "Close");
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.PressureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run test. Pressure level ({Services.Cache.EnvironmentalInfo.Pressure} kPa) is out of range.", "Pressure Warning", "Close");
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.TemperatureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run test. Temperature level ({Services.Cache.EnvironmentalInfo.Temperature} °C) is out of range.", "Temperature Warning", "Close");
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.BatteryCriticallyLow:
                            Services.Dialogs.ShowAlert($"Unable to run test. Battery Level ({Services.Cache.EnvironmentalInfo.BatteryLevel}%) is critically low: ", "Battery Warning", "Close");
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

                if (Services.Dialogs.SecondsProgressDialogShowing())
                {
                    Services.Dialogs.DismissSecondsProgressDialog();
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
                    CacheService.DeviceCheckEnum deviceStatus = Services.Cache.CheckDeviceBeforeTest();

                    switch (deviceStatus)
                    {
                        case CacheService.DeviceCheckEnum.Ready:
                            await Services.DeviceService.Current.StartTest(BreathTestEnum.Training);
                            break;
                        case CacheService.DeviceCheckEnum.DevicePurging:
                            await Services.Dialogs.ShowSecondsProgressAsync($"Device purging..", Services.DeviceService.Current.DeviceReadyCountDown);
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.HumidityOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Humidity level ({Services.Cache.EnvironmentalInfo.Humidity}%) is out of range.", "Humidity Warning", "Close");
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.PressureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Pressure level ({Services.Cache.EnvironmentalInfo.Pressure} kPa) is out of range.", "Pressure Warning", "Close");
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.TemperatureOutOfRange:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Temperature level ({Services.Cache.EnvironmentalInfo.Temperature} °C) is out of range.", "Temperature Warning", "Close");
                            return; // Don't Increment
                        case CacheService.DeviceCheckEnum.BatteryCriticallyLow:
                            Services.Dialogs.ShowAlert($"Unable to run practice test. Battery Level ({Services.Cache.EnvironmentalInfo.BatteryLevel}%) is critically low: ", "Battery Warning", "Close");
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

                if (Services.Dialogs.SecondsProgressDialogShowing())
                {
                    Services.Dialogs.DismissSecondsProgressDialog();
                }
            }

            TutorialIndex = requestedIndex;
            UpdateContent();

            Debug.WriteLine($"***** Current Index = {TutorialIndex}");
        }
    }
}