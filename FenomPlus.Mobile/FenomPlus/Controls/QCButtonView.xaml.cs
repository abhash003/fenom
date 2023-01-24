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
        //public static readonly BindableProperty AssignedProperty = BindableProperty.Create(nameof(Assigned), typeof(bool), typeof(QCButtonView), propertyChanged: AssignedUpdated);
        //public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(QCButtonView), propertyChanged: TitleUpdated);
        //public static readonly BindableProperty StatusProperty = BindableProperty.Create(nameof(Status), typeof(string), typeof(QCButtonView), propertyChanged: StatusUpdated);
        //public static readonly BindableProperty ExpiresProperty = BindableProperty.Create(nameof(Expires), typeof(string), typeof(QCButtonView), propertyChanged: ExpiresUpdated);
        //public static readonly BindableProperty NextTestProperty = BindableProperty.Create(nameof(NextTest), typeof(string), typeof(QCButtonView), propertyChanged: NextTestUpdated);
        //public static readonly BindableProperty ChartDataProperty = BindableProperty.Create(nameof(ChartData), typeof(double[]), typeof(QCButtonView), propertyChanged: ChartDataUpdated);

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

        //public bool Assigned
        //{
        //    get => (bool)this.GetValue(AssignedProperty);
        //    set => this.SetValue(AssignedProperty, value);
        //}

        //public string Title
        //{
        //    get => (string)this.GetValue(TitleProperty);
        //    set => this.SetValue(TitleProperty, value);
        //}

        //public string Status
        //{
        //    get => (string)this.GetValue(StatusProperty);
        //    set => this.SetValue(StatusProperty, value);
        //}

        //public string Expires
        //{
        //    get => (string)this.GetValue(ExpiresProperty);
        //    set => this.SetValue(ExpiresProperty, value);
        //}

        //public string NextTest
        //{
        //    get => (string)this.GetValue(NextTestProperty);
        //    set => this.SetValue(NextTestProperty, value);
        //}

        //public double[] ChartData
        //{
        //    get => (double[])this.GetValue(ChartDataProperty);
        //    set => this.SetValue(ChartDataProperty, value);
        //}




        public QCButtonView()
        {
            InitializeComponent();

            //this.InnerButton.BindingContext = this;
            //this.InnerButton.Clicked += this.OnClicked;
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

        //private static void TitleUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCButtonView qcButton && newValue != null)
        //    {
        //        qcButton.HeaderLabel.Text = (string)newValue;
        //    }
        //}

        //private static void StatusUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCButtonView qcButton && newValue != null)
        //    {
        //        qcButton.StatusLabel.Text = (string)newValue;
        //    }
        //}

        //private static void ExpiresUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCButtonView qcButton && newValue != null)
        //    {
        //        qcButton.ExpiresLabel.Text = (string)newValue;
        //    }
        //}

        //private static void NextTestUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCButtonView qcButton && newValue != null)
        //    {
        //        qcButton.NextTestLabel.Text = (string)newValue;
        //    }
        //}


        //private static void ChartDataUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCButtonView qcButton && newValue != null)
        //    {
        //        //qcButton.NextTestLabel.Text = (string)newValue;
        //    }
        //}

        //private static void AssignedUpdated(object sender, object oldValue, object newValue)
        //{
        //    if (sender is QCButtonView qcButton && newValue != null)
        //    {
        //        qcButton.Assigned = (bool)newValue;
        //    }
        //}


        //private void OnClicked(object sender, EventArgs args)
        //{
        //    Command.Execute(null);
        //    ////this.Clicked?.Invoke(sender, args);
        //    ////}
        //}
    }
}