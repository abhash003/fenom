﻿using FenomPlus.Helpers;
using FenomPlus.ViewModels;
using System;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/overview
    // Documentation:  https://help.syncfusion.com/xamarin/datagrid/export-to-pdf
    // Documentation:  https://help.syncfusion.com/xamarin/pdf-viewer/printing-pdf-files

    public partial class ViewRecentErrorsView : BaseContentPage
    {
        private readonly ViewRecentErrorsViewModel ViewRecentErrorsViewModel;

        public ViewRecentErrorsView()
        {
            InitializeComponent();

            BindingContext = ViewRecentErrorsViewModel = new ViewRecentErrorsViewModel();
            RecentErrorsDataGrid.GridStyle = new CustomGridStyle();

            RecentErrorsDataGrid.Focus();

            //RecentErrorsDataGrid.ExportToPdf or ExportToPdfGrid
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
            ViewRecentErrorsViewModel.OnAppearing();

            // Chunk of code is for optimization
            RecentErrorsDataGrid.Columns.Suspend();
            ViewRecentErrorsViewModel.UpdateRecentErrorsDataCommand.Execute(null);
            RecentErrorsDataGrid.Columns.Resume();
            RecentErrorsDataGrid.RefreshColumns();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewRecentErrorsViewModel.OnDisappearing();
        }

        public override void NewGlobalData()
        {
            base.NewGlobalData();
            ViewRecentErrorsViewModel.NewGlobalData();
        }


    }
}