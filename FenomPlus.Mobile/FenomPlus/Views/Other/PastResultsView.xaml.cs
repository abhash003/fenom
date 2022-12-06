using FenomPlus.Helpers;
using FenomPlus.ViewModels;
using System.IO;
using System;
using Xamarin.Forms;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms.Xaml;
using Syncfusion.SfDataGrid.XForms.Exporting;

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
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PDFExport_Clicked(object sender, EventArgs e)
        {
            SaveAsPdf();
        }

        private void SaveAsPdf()
        {
            DataGridPdfExportingController pdfExport = new DataGridPdfExportingController();

            DataGridPdfExportOption option = new DataGridPdfExportOption();
            option.ExportAllPages = true;
            option.GridLineType = GridLineType.Horizontal;
            option.FitAllColumnsInOnePage = true;

            MemoryStream stream = new MemoryStream();
            var exportToPdf = pdfExport.ExportToPdf(PastResultsDataGrid, option);

            exportToPdf.Save(stream);
            exportToPdf.Close(true);

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "FenomPlus Past Results.pdf");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.WriteAllBytes(filePath, stream.ToArray());

            //if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
            //    Xamarin.Forms.DependencyService.Get<ISaveWindowsPhone>().Save("DataGrid.pdf", "application/pdf", stream);
            //else
            //    Xamarin.Forms.DependencyService.Get<ISave>().Save("DataGrid.pdf", "application/pdf", stream);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PastResultsViewModel.OnAppearing();

            PastResultsViewModel.RefreshPastResultsCommand.Execute(null);

            // Trying to resolve an issue where datagrid doesn't show updated data
            DataPager.Source = null;
            DataPager.Source = PastResultsViewModel.PastResultsData;

            PastResultsDataGrid.ItemsSource = null;
            PastResultsDataGrid.ItemsSource = DataPager.PagedSource;

            //DataPager.Refresh();
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
