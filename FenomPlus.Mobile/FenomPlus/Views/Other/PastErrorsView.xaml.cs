using FenomPlus.Helpers;
using FenomPlus.ViewModels;
using System;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/overview
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/export-to-pdf
    // Documentation:  https://help.syncfusion.com/xamarin/pdf-viewer/printing-pdf-files

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PastErrorsView : BaseContentPage
    {
        private readonly PastErrorsViewModel PastErrorsViewModel;

        public PastErrorsView()
        {
            InitializeComponent();

            BindingContext = PastErrorsViewModel = new PastErrorsViewModel();
            RecentErrorsDataGrid.GridStyle = new CustomGridStyle();

            DataPager.Source = PastErrorsViewModel.RecentErrorsData;
            RecentErrorsDataGrid.ItemsSource = DataPager.PagedSource;
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PDFExport_Clicked(object sender, EventArgs e)
        {
            //DataGridPdfExportingController pdfExport = new DataGridPdfExportingController();
            //MemoryStream stream = new MemoryStream();
            //var exportToPdf = pdfExport.ExportToPdf(this.dataGrid, new DataGridPdfExportOption()
            //{
            //    FitAllColumnsInOnePage = true,
            //});
            //exportToPdf.Save(stream);
            //exportToPdf.Close(true);
            //if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
            //    Xamarin.Forms.DependencyService.Get<ISaveWindowsPhone>().Save("DataGrid.pdf", "application/pdf", stream);
            //else
            //    Xamarin.Forms.DependencyService.Get<ISave>().Save("DataGrid.pdf", "application/pdf", stream);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PastErrorsViewModel.OnAppearing();

            PastErrorsViewModel.RefreshRecentErrorsCommand.Execute(null);

            DataPager.Refresh();
            RecentErrorsDataGrid.RefreshColumns();

            ExitButton.Focus();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            PastErrorsViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            PastErrorsViewModel.NewGlobalData();
        }


    }
}
