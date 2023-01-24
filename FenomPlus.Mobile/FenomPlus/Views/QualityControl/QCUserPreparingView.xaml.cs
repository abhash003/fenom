using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class HumanControlPreparingView : BaseContentPage
    {
        public HumanControlPreparingView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //model.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //model.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            //model.NewGlobalData();
        }
    }
}
