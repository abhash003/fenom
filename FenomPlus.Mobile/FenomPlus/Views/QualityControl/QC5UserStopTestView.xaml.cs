using FenomPlus.ViewModels.QualityControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FenomPlus.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCUserStopTestView : ContentView
    {
        private readonly QCUserStopTestViewModel QCUserStopTestViewModel;

        public QCUserStopTestView()
        {
            InitializeComponent();
            BindingContext = QCUserStopTestViewModel = new QCUserStopTestViewModel();
        }
    }
}