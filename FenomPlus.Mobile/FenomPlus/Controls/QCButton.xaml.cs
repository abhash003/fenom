using Syncfusion.SfDataGrid.XForms.DataPager;
using Syncfusion.XForms.Buttons;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FenomPlus.Controls
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QCButton
    {
        //public event EventHandler Clicked = delegate { };

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(QCButton), propertyChanged: CommandUpdated);
        public ICommand Command
        {
            get => (ICommand)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(QCButton), propertyChanged: CommandParameterUpdated);
        public object CommandParameter
        {
            get => this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty EnabledProperty = BindableProperty.Create(nameof(Enabled), typeof(bool), typeof(QCButton), propertyChanged: EnabledUpdated);
        public bool Enabled
        {
            get => (bool)this.GetValue(EnabledProperty);
            set => this.SetValue(EnabledProperty, value);
        }

        // -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(QCButton), typeof(string), typeof(QCButton), propertyChanged: TitleUpdated);
        public string Title
        {
            get => (string)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty StatusProperty = BindableProperty.Create(nameof(QCButton), typeof(string), typeof(QCButton), propertyChanged: StatusUpdated);
        public string Status
        {
            get => (string)this.GetValue(StatusProperty);
            set => this.SetValue(StatusProperty, value);
        }

        public static readonly BindableProperty ExpiresProperty = BindableProperty.Create(nameof(QCButton), typeof(string), typeof(QCButton), propertyChanged: ExpiresUpdated);
        public string Expires
        {
            get => (string)this.GetValue(ExpiresProperty);
            set => this.SetValue(ExpiresProperty, value);
        }

        public static readonly BindableProperty NextTestProperty = BindableProperty.Create(nameof(QCButton), typeof(string), typeof(QCButton), propertyChanged: NextTestUpdated);
        public string NextTest
        {
            get => (string)this.GetValue(NextTestProperty);
            set => this.SetValue(NextTestProperty, value);
        }

        public static readonly BindableProperty ChartDataProperty = BindableProperty.Create(nameof(QCButton), typeof(double[]), typeof(QCButton), propertyChanged: ChartDataUpdated);
        public double[] ChartData
        {
            get => (double[])this.GetValue(ChartDataProperty);
            set => this.SetValue(ChartDataProperty, value);
        }

// -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------











        public QCButton()
        {
            InitializeComponent();

            //this.InnerButton.BindingContext = this;
            //this.InnerButton.Clicked += this.OnClicked;
        }




        private static void CommandUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue is ICommand newCommand)
            {
                qcButton.InnerButton.Command = newCommand;
            }
        }

        private static void CommandParameterUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue != null)
            {
                qcButton.InnerButton.CommandParameter = newValue;
            }
        }

        private static void EnabledUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue != null)
            {
                qcButton.InnerButton.IsEnabled = (bool)newValue;
            }
        }

        private static void TitleUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue != null)
            {
                qcButton.HeaderLabel.Text = (string)newValue;
            }
        }

        private static void StatusUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue != null)
            {
                qcButton.StatusLabel.Text = (string)newValue;
            }
        }

        private static void ExpiresUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue != null)
            {
                qcButton.ExpiresLabel.Text = (string)newValue;
            }
        }

        private static void NextTestUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue != null)
            {
                qcButton.NextTestLabel.Text = (string)newValue;
            }
        }

        
        private static void ChartDataUpdated(object sender, object oldValue, object newValue)
        {
            if (sender is QCButton qcButton && newValue != null)
            {
                //qcButton.NextTestLabel.Text = (string)newValue;
            }
        }


        //private void OnClicked(object sender, EventArgs args)
        //{
        //    Command.Execute(null);
        //    //this.Clicked?.Invoke(sender, args);
        //}
    }
}