using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace FenomPlus.Controls
{
    public partial class StatusDetailsPopup : Popup
    {
        public StatusDetailsPopup()
        {
            InitializeComponent();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Dismiss(null);
        }
    }
}
