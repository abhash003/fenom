using System;
using System.Collections.Generic;
using System.Threading;
using FenomPlus.SDK.Core.Features;
using FenomPlus.ViewModels;
using Xamarin.Forms;

namespace FenomPlus.Views
{
    public partial class DebugDumpView : BaseContentPage
    {
        private DebugDumpViewModel model;

        public DebugDumpView()
        {
            InitializeComponent();
            BindingContext = model = new DebugDumpViewModel();
            DebugListView.ItemsSource = model.DebugList;
            MessageId.DataSource = model.MessageIdList;
            SubId.DataSource = model.SubIdList;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            model.OnAppearing();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            model.OnDisappearing();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void NewGlobalData()
        {
            base.NewGlobalData();
            model.NewGlobalData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClearDebug(System.Object sender, System.EventArgs e)
        {
            model.DebugList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSendClicked(System.Object sender, System.EventArgs e)
        {
            
            MESSAGE message = new MESSAGE(
                (ID_MESSAGE)Math.Abs(MessageId.SelectedIndex),
                (ID_SUB)Math.Abs(SubId.SelectedIndex),
                (Byte)Math.Abs(Convert.ToByte(Var.Value)));
            
            Services.BleHub.SendMessage(message);
        }
    }
}
