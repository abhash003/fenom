using FenomPlus.Services;
using System;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        public void NotifyViews()
        {
            if (Current?.CurrentPage == null) return;
            try
            {
                ((BaseContentPage)Current.CurrentPage).NewGlobalData();
            } 
            catch(Exception ex) 
            {
                IOC.Services.LogCat.Print(ex);
            }
        }

        public void NotifyViewModels()
        {

        }
    }
}
