﻿using System;
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
        protected override void NewGlobalData()
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
            MESSAGE message = new MESSAGE()
            {
                IDMSG = (ushort)Math.Abs(MessageId.SelectedIndex),
                IDSUB = (ushort)Math.Abs(SubId.SelectedIndex),
                IDVAR = (UInt64)Math.Abs(Convert.ToInt64(Var.Value))
            };
            Services.BleHub.SendMessage(message);
        }
    }
}
