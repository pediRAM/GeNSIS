using GeNSIS.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeNSIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ProjectViewModel m_ProjectViewModel;
        private string m_ProjectName = "Unsaved";
        private void NotifyPropertyChanged(string pPropertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ProjectViewModel ProjectViewModel
        {
            get { return m_ProjectViewModel; }
            set
            {
                if (value == m_ProjectViewModel) return;
                m_ProjectViewModel = value;
                NotifyPropertyChanged(nameof(ProjectViewModel));
            }
        }

        public string ProjectName
        {
            get => m_ProjectName;
            set
            {
                if (value == m_ProjectName) return;
                m_ProjectName = value;
                NotifyPropertyChanged(nameof(ProjectName));
            }
        }

    }
}
