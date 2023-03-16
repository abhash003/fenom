using Syncfusion.XForms.Buttons;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCSettingsButtonView
    {
        //public event EventHandler Clicked = delegate { };

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(QCSettingsButtonView), propertyChanged: CommandUpdated);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(QCSettingsButtonView), propertyChanged: CommandParameterUpdated);
        //public static readonly BindableProperty EnabledProperty = BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(QCSettingsButtonView), propertyChanged: EnabledUpdated);
        //public static readonly BindableProperty ImageNameProperty = BindableProperty.Create(nameof(ImageName), typeof(string), typeof(QCSettingsButtonView), propertyChanged: ImageUpdated);

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

        //public bool Enabled
        //{
        //    get => (bool)this.GetValue(EnabledProperty);
        //    set => this.SetValue(EnabledProperty, value);
        //}

        //public string ImageName
        //{
        //    get => (string)this.GetValue(ImageNameProperty);
        //    set => this.SetValue(ImageNameProperty, value);
        //}


        public QCSettingsButtonView()
        {
            InitializeComponent();

            //this.InnerButton.BindingContext = this;
            //this.InnerButton.Clicked += this.OnClicked;
        }




        private static void CommandUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCSettingsButtonView imageButton && newValue is ICommand newCommand)
            {
                imageButton.InnerButton.Command = newCommand;
            }
        }

        private static void CommandParameterUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCSettingsButtonView imageButton && newValue != null)
            {
                imageButton.InnerButton.CommandParameter = newValue;
            }
        }

        //private static void EnabledUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCSettingsButtonView imageButton && newValue != null)
        //    {
        //        imageButton.InnerButton.IsEnabled = (bool)newValue;
        //    }
        //}

        //private static void ImageUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCSettingsButtonView imageButton && newValue != null)
        //    {
        //        imageButton.ButtonImage.Source = (string)newValue;
        //    }
        //}

        //private void OnClicked(object sender, EventArgs args)
        //{
        //    Command.Execute(null);
        //    //this.Clicked?.Invoke(sender, args);
        //}
    }
}