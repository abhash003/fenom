﻿using FenomPlus.Enums;
using FenomPlus.Helpers;
using FenomPlus.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{
    // Note: Control size by using margins so the size is dynamic to the page at time of viewing

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarigoldProgressWheel
    {
        public static readonly BindableProperty SecondsDurationProperty = BindableProperty.Create("SecondsDuration", typeof(int), typeof(MarigoldProgressWheel), 24);

        public int SecondsDuration
        {
            get => (int)GetValue(SecondsDurationProperty);
            set => SetValue(SecondsDurationProperty, value);
        }

        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create("AutoPlay", typeof(bool), typeof(MarigoldProgressWheel), true);

        public bool AutoPlay
        {
            get => (bool)GetValue(AutoPlayProperty);
            set => SetValue(AutoPlayProperty, value);
        }

        public static readonly BindableProperty RecurringProperty = BindableProperty.Create("Recurring", typeof(bool), typeof(MarigoldProgressWheel), false);
        public bool Recurring 
        {
            get => (bool)GetValue(RecurringProperty);
            set => SetValue(RecurringProperty, value);
        }

        public static readonly BindableProperty ShowTimeProperty = BindableProperty.Create("ShowTime", typeof(bool), typeof(MarigoldProgressWheel), false);

        public bool ShowTime
        {
            get => (bool)GetValue(ShowTimeProperty);
            set => SetValue(ShowTimeProperty, value);
        }


        private readonly List<string> PetalImageFileNames = new List<string>();

        private int PetalIndex;
        private readonly PausableTimer AnimationTimer;

        public MarigoldProgressWheel()
        {
            InitializeComponent();
            IsVisible = false;

            for(int i = 1; i<25; ++i)
            {
                PetalImageFileNames.Add(String.Format("petals_{0:D2}.png", i)); 
            }

            MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[0]);

            AnimationTimer = new PausableTimer(Convert.ToInt32((SecondsDuration * 1000) / PetalImageFileNames.Count));
            AnimationTimer.Elapsed += async (sender, e) => await IncrementMarigoldPetals();

            PetalIndex = 0;
            MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[PetalIndex]);
            MessagingCenter.Subscribe<App>(this, "AppSleeping", _ =>
            {
                AnimationTimer.Pause();
            });

            MessagingCenter.Subscribe<Services.DeviceService.Abstract.Device, string>(this, "NOScore", (sender, arg) =>
            {
                StopAnimation();
            });
        }

        private bool _animation_started = false;
        public void StartOrResumeAnimation()
        {
            if (_animation_started == false)
            {
                PetalIndex = 0;

                Device.BeginInvokeOnMainThread(() =>
                {
                    ActualTimeLabel.Text = string.Empty;
                    IsVisible = true;
                    MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[PetalIndex]);
                });

                AnimationTimer.Start();
                _animation_started = true;
            }
            else
            {
                AnimationTimer.Resume();
            }
        }
        public void StopAnimation()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsVisible = false;
                PetalIndex = 0;
            });

            AnimationTimer.Stop();
            _animation_started = false;
        }

        private Task IncrementMarigoldPetals()
        {
            if (PetalIndex < PetalImageFileNames.Count - 1 || Recurring)
            {
                ++ PetalIndex;
                if (Recurring && PetalIndex + 1 == PetalImageFileNames.Count)
                {
                    PetalIndex = 0;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[PetalIndex]);
                });
            }
            else
            {
                StopAnimation();
            }

            return Task.CompletedTask;
        }
    }
}