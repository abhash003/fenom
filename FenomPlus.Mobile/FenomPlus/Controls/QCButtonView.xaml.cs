using Syncfusion.SfDataGrid.XForms.DataPager;
using Syncfusion.XForms.Buttons;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCButtonView
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(QCButtonView), propertyChanged: CommandUpdated);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(QCButtonView), propertyChanged: CommandParameterUpdated);

        public ICommand Command
        {
            get => (ICommand)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        public QCButtonView()
        {
            InitializeComponent();
        }


        private static void CommandUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButtonView qcButton && newValue is ICommand newCommand)
            {
                qcButton.InnerButton.Command = newCommand;
            }
        }

        private static void CommandParameterUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButtonView qcButton && newValue != null)
            {
                qcButton.InnerButton.CommandParameter = newValue;
            }
        }

        void OnChartTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            var imageSender = (Image)sender;
        }
    }
}