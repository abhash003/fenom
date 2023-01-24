using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Controls
{
    public partial class QCSettingsButtonViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _imageName = string.Empty;

        public QCSettingsButtonViewModel()
        {
            
        }

        [RelayCommand]
        private void ButtonPress()
        {

        }
    }
}
