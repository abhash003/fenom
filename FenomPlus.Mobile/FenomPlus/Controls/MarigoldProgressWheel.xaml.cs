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

        public static readonly BindableProperty AutoHideProperty = BindableProperty.Create("AutoHide", typeof(bool), typeof(MarigoldProgressWheel), true);

        public bool AutoHide
        {
            get => (bool)GetValue(AutoHideProperty);
            set => SetValue(AutoHideProperty, value);
        }

        public static readonly BindableProperty ShowTimeProperty = BindableProperty.Create("ShowTime", typeof(bool), typeof(MarigoldProgressWheel), false);

        public bool ShowTime
        {
            get => (bool)GetValue(ShowTimeProperty);
            set => SetValue(ShowTimeProperty, value);
        }


        private readonly List<string> PetalImageFileNames = new List<string>();

        private int PetalIndex;
        private readonly Timer AnimationTimer;

        public MarigoldProgressWheel()
        {
            InitializeComponent();
            IsVisible = false;

            PetalImageFileNames.Add("petals_01.png");
            PetalImageFileNames.Add("petals_02.png");
            PetalImageFileNames.Add("petals_03.png");
            PetalImageFileNames.Add("petals_04.png");
            PetalImageFileNames.Add("petals_05.png");
            PetalImageFileNames.Add("petals_06.png");
            PetalImageFileNames.Add("petals_07.png");
            PetalImageFileNames.Add("petals_08.png");
            PetalImageFileNames.Add("petals_09.png");
            PetalImageFileNames.Add("petals_10.png");
            PetalImageFileNames.Add("petals_11.png");
            PetalImageFileNames.Add("petals_12.png");
            PetalImageFileNames.Add("petals_13.png");
            PetalImageFileNames.Add("petals_14.png");
            PetalImageFileNames.Add("petals_15.png");
            PetalImageFileNames.Add("petals_16.png");
            PetalImageFileNames.Add("petals_17.png");
            PetalImageFileNames.Add("petals_18.png");
            PetalImageFileNames.Add("petals_19.png");
            PetalImageFileNames.Add("petals_20.png");
            PetalImageFileNames.Add("petals_21.png");
            PetalImageFileNames.Add("petals_22.png");
            PetalImageFileNames.Add("petals_23.png");
            PetalImageFileNames.Add("petals_24.png");

            MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[0]);

            AnimationTimer = new Timer(Convert.ToInt32((SecondsDuration * 1000) / PetalImageFileNames.Count));
            AnimationTimer.Elapsed += async (sender, e) => await IncrementMarigoldPetals();

            PetalIndex = 0;
            MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[PetalIndex]);

            if (AutoPlay)
            {
                StartAnimation();
            }
        }

        public void StartAnimation()
        {
            PetalIndex = 0;

            Device.BeginInvokeOnMainThread(() =>
            {
                ActualTimeLabel.Text = string.Empty;
                IsVisible = true;
                MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[PetalIndex]);
            });

            AnimationTimer.Start();
        }

        public void StopAnimation()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsVisible = false;

                PetalIndex = PetalImageFileNames.Count - 1;
                MarigoldProgressImage.Source = ImageSource.FromFile(PetalImageFileNames[PetalIndex]);
            });

            AnimationTimer.Stop();
        }

        private Task IncrementMarigoldPetals()
        {
            if (PetalIndex < PetalImageFileNames.Count - 1)
            {
                PetalIndex += 1;

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