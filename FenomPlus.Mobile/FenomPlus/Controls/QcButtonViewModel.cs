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
    public partial class QcButtonViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _assigned;

        [ObservableProperty] 
        private string _title = "QC User";

        [ObservableProperty]
        private string _status = "Expired";

        [ObservableProperty]
        private string _expires = "DD MMM YYYY";

        [ObservableProperty]
        private string _nextTest = "HH:MM:SS";

        [ObservableProperty]
        private string _chart = "bar_chart";

        public QcButtonViewModel()
        {
            
        }
    }
}
