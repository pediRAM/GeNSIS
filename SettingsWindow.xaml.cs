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


namespace GeNSIS
{
    using GeNSIS.Core.Models;
    using System.Windows;
    using System.Windows.Forms;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private FolderBrowserDialog m_FBD = new FolderBrowserDialog();

        public SettingsWindow()
        {
            InitializeComponent();
            Title = "Settings";
        }

        public SettingsWindow(IAppConfig pAppConfig) : this()
        {
            Config = new AppConfigVM(true);
            Config.UpdateValues(pAppConfig);
            Config.HasUnsavedChanges = false;
            DataContext = Config;
        }

        public AppConfigVM Config { get; set; }

        private void OnSelectGeNsisProjectFolderClicked(object sender, RoutedEventArgs e)
        {
            if (m_FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Config.GeNSISProjectsDirectory = m_FBD.SelectedPath;
        }

        private void OnSelectScriptFolderClicked(object sender, RoutedEventArgs e)
        {
            if (m_FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Config.ScriptsDirectory = m_FBD.SelectedPath;
        }

        private void OnSelectNsisFolderClicked(object sender, RoutedEventArgs e)
        {
            if (m_FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Config.NsisInstallationDirectory = m_FBD.SelectedPath;
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
