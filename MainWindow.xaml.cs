/***************************************************************************************
* GeNSIS - a free and open source NSIS installer script generator tool.                *
* Copyright (C) 2023 Pedram Ganjeh Hadidi                                              *
*                                                                                      *
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
using GeNSIS.Core.Extensions;
using GeNSIS.Core.TextGenerators;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;


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
        private OpenFileDialog m_OpenIconDialog = new OpenFileDialog();
        private SaveFileDialog m_SaveFileDialog = new SaveFileDialog();
        private FolderBrowserDialog m_FolderBrowserDialog = new FolderBrowserDialog();

        private void NotifyPropertyChanged(string pPropertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));

        public MainWindow()
        {
            AppData = new AppDataViewModel(true);
            

            InitializeComponent();

            Title = $"GeNSIS {AsmConst.VERSION}";
            editor.SyntaxHighlighting = XshdLoader.LoadHighlightingDefinitionOrNull("nsis.xshd");
            m_OpenFileDialog.Multiselect = true;
            m_OpenIconDialog.Filter = "Icon files|*.ico";
            m_OpenIconDialog.Multiselect = false;
            m_SaveFileDialog.Filter = "NSIS files|*.nsi";
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

        private void OnLoadIconClicked(object sender, RoutedEventArgs e)
        {
            if (m_OpenIconDialog.ShowDialog() == true)
            {
                try
                {
                    var fi = new FileInfo(m_OpenIconDialog.FileName);
                    if (fi.Extension.Equals(".ico", StringComparison.InvariantCultureIgnoreCase) && fi.Length > 0)
                    {
                        if (! AppData.Files.Contains(m_OpenIconDialog.FileName))
                            AppData.Files.Add(m_OpenIconDialog.FileName);
                    }
                    AppData.AppIcon = m_OpenIconDialog.FileName;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
            }

        }

        private void OnAddDirectoryClicked(object sender, RoutedEventArgs e)
        {
            if (m_FolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AppData.Directories.Add(m_FolderBrowserDialog.SelectedPath);
            }
        }
        private string m_GeneratedNsisPath;
        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            var validator = new Validator();
            ValidationError error = null;
            if (!validator.IsValid(AppData, out error)) 
                return;

            var g = new NsisGenerator();
            var nsisCode = g.Generate(AppData, new TextGeneratorOptions() { EnableComments = true, EnableLogs = true });
            if (m_SaveFileDialog.ShowDialog() != true)
                return;

            m_GeneratedNsisPath = m_SaveFileDialog.FileName;
            File.WriteAllText(m_SaveFileDialog.FileName, editor.Text, encoding: System.Text.Encoding.UTF8);
            editor.Text = nsisCode;

        }

        private void OnCompileScript(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(m_GeneratedNsisPath))
            {
                MessageBox.Show("Path to currently generated NSIS script is empty!\nPlease generate the file in the previous tab first!", "No *.nsi file to compile!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!File.Exists(m_GeneratedNsisPath))
            {
                MessageBox.Show("Generated NSIS script not found!", "File not found!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var makeNsisExePath = @"C:\Program Files (x86)\NSIS\makensisw.exe";
            var pi = new ProcessStartInfo($"\"{makeNsisExePath}\"", $"\"{m_GeneratedNsisPath}\"");
            pi.UseShellExecute = false;
            
            var proc = new Process();
            proc.StartInfo = pi;
            _ = proc.Start();
        }
    }
}
