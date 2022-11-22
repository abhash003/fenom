using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace FenomPlus.Models
{
    public partial class SensorStatus : ObservableObject
    {
        [ObservableProperty]
        private string imageName = string.Empty;

        [ObservableProperty] 
        private string _value = string.Empty;

        [ObservableProperty]
        private Color _color = Xamarin.Forms.Color.Green;

        [ObservableProperty] 
        private string _label = string.Empty;
    }
}
