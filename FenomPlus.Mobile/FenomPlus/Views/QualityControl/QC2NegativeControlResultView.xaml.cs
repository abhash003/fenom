using FenomPlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FenomPlus.ViewModels.QualityControl;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCNegativeControlResultView : ContentView
    {
        private readonly QCNegativeControlResultViewModel QCNegativeControlResultViewModel;

        public QCNegativeControlResultView()
        {
            InitializeComponent();
            BindingContext = QCNegativeControlResultViewModel = new QCNegativeControlResultViewModel();
        }
    }
}