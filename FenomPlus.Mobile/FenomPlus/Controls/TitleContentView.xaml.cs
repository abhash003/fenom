using System;
using System.Collections.Generic;
using System.ComponentModel;
using FenomPlus.Interfaces;
using FenomPlus.ViewModels;
using TinyIoC;
using Xamarin.Forms;

namespace FenomPlus.Controls
{
    public partial class TitleContentView : ContentView
    {
        public static TinyIoCContainer Container => TinyIoCContainer.Current;
        private TitleContentViewModel TitleContentViewModel;

        public TitleContentView()
        {
            InitializeComponent();

            // Use a single app wide instance
            TitleContentViewModel = Container.Resolve<TitleContentViewModel>();
            BindingContext = TitleContentViewModel;
        }

        private void ImageButton_OnClicked(object sender, EventArgs e)
        {

        }
    }
}
