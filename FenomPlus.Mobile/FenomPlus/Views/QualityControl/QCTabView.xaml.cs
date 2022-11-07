using System;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class QCTabView : BaseContentPage
    {
        public QCTabView()
        {
            InitializeComponent();

            // Not necessary to set Binding Context to a ViewModel
            // This page only handles UI interactions

            Tab1Content.IsVisible = true;
            Tab2Content.IsVisible = false;
            Tab3Content.IsVisible = false;

            UpdateTabButtonBorder();

            BindingContext = new BaseViewModel();
        }

        public void Tab1ButtonClicked(object sender, EventArgs eventArgs)
        {
            Tab1Content.IsVisible = true;
            Tab2Content.IsVisible = false;
            Tab3Content.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void Tab2ButtonClicked(object sender, EventArgs eventArgs)
        {
            Tab1Content.IsVisible = false;
            Tab2Content.IsVisible = true;
            Tab3Content.IsVisible = false;

            UpdateTabButtonBorder();
        }

        public void Tab3ButtonClicked(object sender, EventArgs eventArgs)
        {
            Tab1Content.IsVisible = false;
            Tab2Content.IsVisible = false;
            Tab3Content.IsVisible = true;

            UpdateTabButtonBorder();
        }

        private void UpdateTabButtonBorder()
        {
            Tab1ButtonBorder.IsVisible = Tab1Content.IsVisible;
            Tab2ButtonBorder.IsVisible = Tab2Content.IsVisible;
            Tab3ButtonBorder.IsVisible = Tab3Content.IsVisible;
        }
    }
}