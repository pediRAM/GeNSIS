namespace GeNSIS.UI
{
    using System.ComponentModel;
    using System.Windows;


    /// <summary>
    /// Interaction logic for CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : Window, INotifyPropertyChanged
    {
        private FrameworkElement m_FWElement;

        private string m_WindowTitle;
        private string m_DialogTitle;
        private string m_DialogText;


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));

        public CustomDialog()
        {
            InitializeComponent();
        }

        public string WindowTitle
        {
            get { return m_WindowTitle; }
            set
            {
                if (value == m_WindowTitle) return;
                m_WindowTitle = value;
                NotifyPropertyChanged(nameof(WindowTitle));
            }
        }

        public string DialogTitle
        {
            get { return m_DialogTitle; }
            set
            {
                if (value == m_DialogTitle) return;
                m_DialogTitle = value;
                NotifyPropertyChanged(nameof(DialogTitle));
            }
        }

        public string DialogText
        {
            get { return m_DialogText; }
            set
            {
                if (value == m_DialogText) return;
                m_DialogText = value;
                NotifyPropertyChanged(nameof(DialogText));
            }
        }

        public FrameworkElement FWElement
        {
            get { return m_FWElement; }
            set
            {
                if (value == m_FWElement) return;
                m_FWElement = value;
                NotifyPropertyChanged(nameof(FWElement));
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e) => DialogResult = false;

        private void OnOKClicked(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
