using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class HumanControlPassedView : BaseContentPage
    {
        private readonly HumanControlPassedViewModel HumanControlPassedViewModel;

        public HumanControlPassedView()
        {
            InitializeComponent();
            BindingContext = HumanControlPassedViewModel = new HumanControlPassedViewModel();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
        }
    }
}
