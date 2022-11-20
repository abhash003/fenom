using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.ViewModels;

namespace FenomPlus.Controls
{
    public partial class StatusButtonViewModel :BaseViewModel
    {
        [ObservableProperty]
        private string _header;

        [ObservableProperty]
        private string _imagePath;

        [ObservableProperty]
        private string _value;

        [ObservableProperty]
        private string _label;

        [ObservableProperty]
        private string _description = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.Ut enim.";

        [ObservableProperty]
        private string _buttonText;

        [ObservableProperty]
        private Color _color;

        [ObservableProperty]
        private string _helpLink = "https://www.caireinc.com/";

        [ObservableProperty]
        private bool _displayPopup;

        public StatusButtonViewModel()
        {
            
        }

        [RelayCommand]
        private void ButtonPress()
        {

        }
    }
}
