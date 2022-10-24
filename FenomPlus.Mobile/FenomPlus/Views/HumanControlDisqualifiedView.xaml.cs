using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class HumanControlDisqualifiedView : BaseContentPage
    {
        private HumanControlDisqualifiedViewModel model;

        public HumanControlDisqualifiedView()
        {
            InitializeComponent();
            BindingContext = model = new HumanControlDisqualifiedViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnFinish(System.Object sender, System.EventArgs e)
        {
            await Services.Navigation.QualityControlView();
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
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }
    }
}
