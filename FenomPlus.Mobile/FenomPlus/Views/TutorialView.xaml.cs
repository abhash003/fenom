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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNext(object sender, EventArgs e)
        {
            if (model.TutorialIndex + 1 < model.Tutorials.Count)
            {
                GotoPosition(model.TutorialIndex + 1);
            } else
            {
                model.TutorialIndex = model.Tutorials.Count;
            }
            model.UpdateButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBack(object sender, EventArgs e)
        {
            if(model.TutorialIndex >= model.Tutorials.Count)
            {
                model.TutorialIndex = model.Tutorials.Count;
            }
            if (model.TutorialIndex > 0)
            {
                GotoPosition(model.TutorialIndex - 1);
            }
            model.UpdateButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void GotoPosition(int position)
        {
            model.TutorialIndex = position;
            //carousel.Position = position;
            model.Header = $"Step {model.TutorialIndex + 1}";
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
            GotoPosition(0);
            model.OnAppearing();
            model.UpdateButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
            // force refresh here to zero
            //GotoPosition(0);
            model.UpdateButtons();
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