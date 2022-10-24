using System;
using System.Collections.Generic;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Controls
{
    public partial class TitleContentView : ContentView
    {
        public TitleContentView()
        {
            InitializeComponent();
            BindingContext = new TitleContentViewModel();
        }
    }
}
