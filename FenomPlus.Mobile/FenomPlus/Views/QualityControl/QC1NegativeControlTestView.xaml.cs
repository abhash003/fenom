using FenomPlus.ViewModels;
using FenomPlus.ViewModels.QualityControl;
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
    public partial class QCNegativeControlTestView : ContentView
    {
        private readonly QCNegativeControlTestViewModel QCNegativeControlTestViewModel;

        public QCNegativeControlTestView()
        {
            InitializeComponent();
            BindingContext = QCNegativeControlTestViewModel = new QCNegativeControlTestViewModel();
        }
    }
}