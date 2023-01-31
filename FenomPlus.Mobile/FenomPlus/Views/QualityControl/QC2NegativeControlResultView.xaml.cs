using FenomPlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCNegativeControlResultView : ContentView
    {
        private readonly QualityControlViewModel QualityControlViewModel;

        public QCNegativeControlResultView()
        {
            InitializeComponent();
            BindingContext = QualityControlViewModel = new QualityControlViewModel();
        }
    }
}