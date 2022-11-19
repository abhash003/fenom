﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.ViewModels;

namespace FenomPlus.Controls
{
    public partial class StatusInfoBoxViewModel :BaseViewModel
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
        private string _buttonText;

        [ObservableProperty]
        private Color _color;

        public StatusInfoBoxViewModel()
        {
            
        }

        [RelayCommand]
        private void ButtonPress()
        {

        }
    }
}
