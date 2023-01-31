using FenomPlus.ViewModels;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserTestView : BaseContentPage
    {
        private readonly QCUserTestViewModel QCUserTestViewModel;

        public QCUserTestView()
        {
            InitializeComponent();
            BindingContext = QCUserTestViewModel = new QCUserTestViewModel();
        }
    }
}
