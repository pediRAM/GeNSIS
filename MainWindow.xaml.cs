/***************************************************************************************
* This file is part of GeNSIS.                                                         *
*                                                                                      *
* GeNSIS is free software: you can redistribute it and/or modify it under the terms    *
* of the GNU General Public License as published by the Free Software Foundation,      *
* either version 3 of the License, or any later version.                               *
*                                                                                      *
* GeNSIS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;  *
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR     *
* PURPOSE. See the GNU General Public License for more details.                        *
*                                                                                      *
* You should have received a copy of the GNU General Public License along with GeNSIS. *
* If not, see <https://www.gnu.org/licenses/>.                                         *
****************************************************************************************/


using GeNSIS.Core;
using GeNSIS.Core.Commands;
using GeNSIS.Core.Extensions;
using GeNSIS.Core.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace GeNSIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AppDataViewModel m_AppDataViewModel;
        private string m_ProjectName = "Unsaved";
        private OpenFileDialog m_OpenFileDialog = new OpenFileDialog();
        private FolderBrowserDialog m_FolderBrowserDialog = new FolderBrowserDialog();

        private void NotifyPropertyChanged(string pPropertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));

        public MainWindow()
        {
            InitializeComponent();
            Title = $"GeNSIS {AsmConst.VERSION}";
            editor.SyntaxHighlighting = XshdLoader.LoadHighlighting("nsis.xshd");
            AppData = new AppData().ToViewModel();
            m_OpenFileDialog.Multiselect = true;
            
            DataContext = AppData;
        }

        public AppDataViewModel AppData
        {
            get { return m_AppDataViewModel; }
            set
            {
                if (value == m_AppDataViewModel) 
                    return;
                m_AppDataViewModel = value;
                NotifyPropertyChanged(nameof(AppData));
            }
        }

        public string ProjectName
        {
            get => m_ProjectName;
            set
            {
                if (value == m_ProjectName) 
                    return;
                m_ProjectName = value;
                NotifyPropertyChanged(nameof(ProjectName));
            }
        }

        private void OnAddFilesClicked(object sender, RoutedEventArgs e)
        {
            if (m_OpenFileDialog.ShowDialog().Value != true) 
                return;

            AppData.Files.AddRange(m_OpenFileDialog.FileNames);
        }

    }
}
