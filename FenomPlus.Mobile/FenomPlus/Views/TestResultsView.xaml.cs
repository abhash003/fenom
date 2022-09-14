using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class TestResultsView : BaseContentPage
    {
        private TestResultsViewModel model;

        public TestResultsView()
        {
            InitializeComponent();
            BindingContext = model = new TestResultsViewModel();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Finish_Pressed(System.Object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync(new ShellNavigationState($"///{nameof(ChooseTestView)}"), false);
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

