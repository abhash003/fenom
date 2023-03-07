using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{
    // Note: Control size by using margins so the size is dynamic to the page at time of viewing

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarigoldActivityWheel
    {
        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create("AutoPlay", typeof(bool), typeof(MarigoldProgressWheel), true);

        public bool AutoPlay
        {
            get => (bool)GetValue(AutoPlayProperty);
            set => SetValue(AutoPlayProperty, value);
        }

        private bool IsRunning;

        public MarigoldActivityWheel()
        {
            InitializeComponent();

            if (AutoPlay)
            {
                _ = StartImageRotation();
            }
        }

        private async Task StartImageRotation()
        {
            await MarigoldRotatingImage.RotateTo(0, 0);

            IsRunning = true;

            while (IsRunning)
            {
                await MarigoldRotatingImage.RotateTo(360, 3000);
                await MarigoldRotatingImage.RotateTo(0, 0);
            }
        }

        private void StopImageRotation()
        {
            IsRunning = false;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsVisible))
            {
                if (IsVisible)
                {
                    if (AutoPlay)
                    {
                        _ = StartImageRotation();
                    }
                }
                else
                {
                    StopImageRotation();
                }
            }
        }
        
    }

}