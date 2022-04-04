﻿using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class ChooseAgeView : BaseContentPage
    {
        private ChooseAgeViewModel model;

        public ChooseAgeView()
        {
            InitializeComponent();
            BindingContext = model = new ChooseAgeViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }
    }
}
