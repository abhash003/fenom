using System;
using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class TutorialView : BaseContentPage
    {
        private TutorialViewModel model;

        public TutorialView()
        {
            InitializeComponent();
            BindingContext = model = new TutorialViewModel();
            carousel.ItemsSource = model.Tutorials;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNext(object sender, EventArgs e)
        {
            if (model.TutorialPosition + 1 < model.Tutorials.Count)
            {
                GotoPostion(model.TutorialPosition + 1);
            } else
            {
                model.TutorialPosition = model.Tutorials.Count;
            }
            model.UpdateViews();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBack(object sender, EventArgs e)
        {
            if(model.TutorialPosition >= model.Tutorials.Count)
            {
                model.TutorialPosition = model.Tutorials.Count;
            }
            if (model.TutorialPosition > 0)
            {
                GotoPostion(model.TutorialPosition - 1);
            }
            model.UpdateViews();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postion"></param>
        public void GotoPostion(int postion)
        {
            model.TutorialPosition = postion;
            carousel.Position = postion;
            model.Header = $"Step {carousel.Position + 1}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCancelled(object sender, EventArgs e)
        {
            await Services.Navigation.ChooseTestView();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GotoPostion(0);
            model.OnAppearing();
            model.UpdateViews();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
            // force refresh here to zero
            //GotoPostion(0);
            model.UpdateViews();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }
    }
}