using System;
using Xamarin.Forms;

namespace FenomPlus.Models
{
    public class SensorStatus : BaseModel
    {
        private ImageSource image;
        public ImageSource Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged("Image");
            }
        }

        private string value;
        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        private Color color;
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }

    }
}
