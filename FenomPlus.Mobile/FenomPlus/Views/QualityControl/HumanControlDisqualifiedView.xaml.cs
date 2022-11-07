using FenomPlus.ViewModels;

namespace FenomPlus.Views
{
    public partial class HumanControlDisqualifiedView : BaseContentPage
    {
        private readonly HumanControlDisqualifiedViewModel HumanControlDisqualifiedViewModel;

        public HumanControlDisqualifiedView()
        {
            InitializeComponent();
            BindingContext = HumanControlDisqualifiedViewModel = new HumanControlDisqualifiedViewModel();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            HumanControlDisqualifiedViewModel.NewGlobalData();
        }
    }
}
