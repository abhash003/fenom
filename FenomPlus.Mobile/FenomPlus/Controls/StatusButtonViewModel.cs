using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.ViewModels;

namespace FenomPlus.Controls
{
    public partial class StatusButtonViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _header;

        [ObservableProperty]
        private string _imagePath;

        [ObservableProperty]
        private string _value;

        [ObservableProperty] 
        private Color _valueColor;

        [ObservableProperty]
        private string _label;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _buttonText;

        [ObservableProperty]
        private string _helpLink = "https://www.caireinc.com/";

        public StatusButtonViewModel()
        {
        }

    }
}
