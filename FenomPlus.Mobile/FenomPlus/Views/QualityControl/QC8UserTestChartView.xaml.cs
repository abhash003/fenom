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
    public partial class QCUserTestChartView : ContentView
    {
        private readonly QCUserTestChartViewModel QCUserTestChartViewModel;

        public QCUserTestChartView()
        {
            InitializeComponent();
            BindingContext = QCUserTestChartViewModel = new QCUserTestChartViewModel();
        }
    }
}