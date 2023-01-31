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
    public partial class QCUserTestResultView : ContentView
    {
        private readonly QCUserTestResultViewModel QCUserTestResultViewModel;

        public QCUserTestResultView()
        {
            InitializeComponent();
            BindingContext = QCUserTestResultViewModel = new QCUserTestResultViewModel();
        }
    }
}