using Syncfusion.XForms.Buttons;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusInfoButton
    {
        //public event EventHandler Clicked = delegate { };

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(StatusInfoButton), propertyChanged: CommandUpdated);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(StatusInfoButton), propertyChanged: CommandParameterUpdated);
        public static readonly BindableProperty EnabledProperty = BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(StatusInfoButton), propertyChanged: EnabledUpdated);

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

        public bool Enabled
        {
            get => (bool)this.GetValue(EnabledProperty);
            set => this.SetValue(EnabledProperty, value);
        }


        public StatusInfoButton()
        {
            InitializeComponent();

            //this.InnerButton.BindingContext = this;
            //this.InnerButton.Clicked += this.OnClicked;
        }




        private static void CommandUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is StatusInfoButton statusInfoButton && newValue is ICommand newCommand)
            {
                statusInfoButton.InnerButton.Command = newCommand;
            }
        }

        private static void CommandParameterUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is StatusInfoButton statusInfoButton && newValue != null)
            {
                statusInfoButton.InnerButton.CommandParameter = newValue;
            }
        }

        private static void EnabledUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is StatusInfoButton statusInfoButton && newValue != null)
            {
                statusInfoButton.InnerButton.IsEnabled = (bool)newValue;
            }
        }

        //private void OnClicked(object sender, EventArgs args)
        //{
        //    Command.Execute(null);
        //    //this.Clicked?.Invoke(sender, args);
        //}
    }
}