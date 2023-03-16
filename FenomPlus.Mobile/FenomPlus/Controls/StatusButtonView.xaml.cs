using Syncfusion.XForms.Buttons;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusButtonView
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(StatusButtonView), propertyChanged: CommandUpdated);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(StatusButtonView), propertyChanged: CommandParameterUpdated);

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

        public StatusButtonView()
        {
            InitializeComponent();
        }

        private static void CommandUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is StatusButtonView statusInfoButton && newValue is ICommand newCommand)
            {
                statusInfoButton.InnerButton.Command = newCommand;
            }
        }

        private static void CommandParameterUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is StatusButtonView statusInfoButton && newValue != null)
            {
                statusInfoButton.InnerButton.CommandParameter = newValue;
            }
        }
    }
}