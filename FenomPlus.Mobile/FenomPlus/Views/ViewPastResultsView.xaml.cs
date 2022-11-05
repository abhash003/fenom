using FenomPlus.Helpers;
using FenomPlus.ViewModels;
using System.IO;
using System;
using Xamarin.Forms;
using Syncfusion.SfDataGrid.XForms;

namespace FenomPlus.Views
{
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/overview
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/export-to-pdf
    // Documentation:  https://help.syncfusion.com/xamarin/pdf-viewer/printing-pdf-files


    public partial class ViewPastResultsView : BaseContentPage
    {
        private ViewPastResultsViewModel model;

        public ViewPastResultsView()
        {
            InitializeComponent();

            BindingContext = model = new ViewPastResultsViewModel();
            PastResultsDataGrid.GridStyle = new CustomGridStyle();

            PastResultsDataGrid.Focus();

            //dataGrid.AllowSorting
            //dataGrid.ExportToPdf or ExportToPdfGrid
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

            PastResultsDataGrid.Columns.Suspend();

            model.UpdatePastResultsDataCommand.Execute(null);

            // Add or Remove More columns
            PastResultsDataGrid.Columns.Resume();
            PastResultsDataGrid.RefreshColumns();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
