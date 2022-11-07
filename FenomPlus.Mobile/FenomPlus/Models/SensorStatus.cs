using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace FenomPlus.Models
{
    public partial class SensorStatus : ObservableObject
    {
        [ObservableProperty]
        private ImageSource image;

        [ObservableProperty]
        private string _value;

        [ObservableProperty]
        private Color _color;
    }
}
