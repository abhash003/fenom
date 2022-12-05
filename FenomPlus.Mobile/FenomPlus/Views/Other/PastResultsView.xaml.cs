using FenomPlus.Helpers;
using FenomPlus.ViewModels;
using System.IO;
using System;
using Xamarin.Forms;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Views
{
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/overview
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/export-to-pdf
    // Documentation:  https://help.syncfusion.com/xamarin/pdf-viewer/printing-pdf-files

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PastResultsView : BaseContentPage
    {
        private readonly PastResultsViewModel PastResultsViewModel;

        public PastResultsView()
        {
            InitializeComponent();

            BindingContext = PastResultsViewModel = new PastResultsViewModel();
            PastResultsDataGrid.GridStyle = new CustomGridStyle();

            PastResultsDataGrid.Focus();

            //PastResultsDataGrid.ExportToPdf or ExportToPdfGrid
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PDFExport_Clicked(object sender, EventArgs e)
        {
            //PastResultsDataGrid


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

            PastResultsViewModel.RefreshPastResultsCommand.Execute(null);

            DataPager.Source = PastResultsViewModel.PastResultsData;
            PastResultsDataGrid.ItemsSource = DataPager.PagedSource;
            DataPager.TabIndex = 0;
            DataPager.Refresh();
            PastResultsDataGrid.RefreshColumns();
        }



        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            PastResultsViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            PastResultsViewModel.NewGlobalData();
        }


    }
}
