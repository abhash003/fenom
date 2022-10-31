using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{
    // Note: Control size by using margins so the size is dynamic to the page at time of viewing

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarigoldProgressWheel
    {
        public static readonly BindableProperty SecondsDurationProperty =
            BindableProperty.Create("SecondsDuration", typeof(int), typeof(MarigoldProgressWheel), 23);

        public int SecondsDuration
        {
            get => (int)GetValue(SecondsDurationProperty);
            set => SetValue(SecondsDurationProperty, value);
        }

        public static readonly BindableProperty AutoPlayProperty =
            BindableProperty.Create("AutoPlay", typeof(bool), typeof(MarigoldProgressWheel), true);

        public bool AutoPlay
        {
            get => (bool)GetValue(AutoPlayProperty);
            set => SetValue(AutoPlayProperty, value);
        }

        public static readonly BindableProperty AutoHideProperty =
            BindableProperty.Create("AutoHide", typeof(bool), typeof(MarigoldProgressWheel), true);

        public bool AutoHide
        {
            get => (bool)GetValue(AutoHideProperty);
            set => SetValue(AutoHideProperty, value);
        }


        private readonly List<string> SegmentImages = new List<string>();

        private int SegmentIndex;

        public MarigoldProgressWheel()
        {
            InitializeComponent();

            SegmentImages.Add("petals_01.png");
            SegmentImages.Add("petals_02.png");
            SegmentImages.Add("petals_03.png");
            SegmentImages.Add("petals_04.png");
            SegmentImages.Add("petals_05.png");
            SegmentImages.Add("petals_06.png");
            SegmentImages.Add("petals_07.png");
            SegmentImages.Add("petals_08.png");
            SegmentImages.Add("petals_09.png");
            SegmentImages.Add("petals_10.png");
            SegmentImages.Add("petals_11.png");
            SegmentImages.Add("petals_12.png");
            SegmentImages.Add("petals_13.png");
            SegmentImages.Add("petals_14.png");
            SegmentImages.Add("petals_15.png");
            SegmentImages.Add("petals_16.png");
            SegmentImages.Add("petals_17.png");
            SegmentImages.Add("petals_18.png");
            SegmentImages.Add("petals_19.png");
            SegmentImages.Add("petals_20.png");
            SegmentImages.Add("petals_21.png");
            SegmentImages.Add("petals_22.png");
            SegmentImages.Add("petals_23.png");
            SegmentImages.Add("petals_24.png");

            if (AutoPlay)
            {
                StartAnimation();
            }
        }

        private Timer AnimationTimer;

        public void StartAnimation()
        {
            SegmentIndex = 0;
            ProgressStepLabel.Text = SegmentIndex.ToString();

            MarigoldProgressImage.Source = ImageSource.FromFile(SegmentImages[SegmentIndex]);

            int milliseconds = Convert.ToInt32((SecondsDuration * 1000) / SegmentImages.Count);

            // Use System.Timer - Device.Timer not working properly
            AnimationTimer = new Timer(TickTimer, 0, milliseconds, milliseconds);
        }

        private void TickTimer(object state)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                SegmentIndex += 1;
                Debug.WriteLine($"Tick #{SegmentIndex}");

                if (SegmentIndex <= SegmentImages.Count - 1)
                {
                    MarigoldProgressImage.Source = ImageSource.FromFile(SegmentImages[SegmentIndex]);
                    ProgressStepLabel.Text = SegmentIndex.ToString(); ;
                }
                else
                {
                    AnimationTimer.Dispose();
                }
            });
        }

        public void StopAnimation()
        {
            SegmentIndex = 0;
            MarigoldProgressImage.Source = string.Empty;
            AnimationTimer?.Dispose();
        }
    }
}

