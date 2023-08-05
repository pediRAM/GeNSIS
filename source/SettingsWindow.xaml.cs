/*
GeNSIS (GEnerates NullSoft Installer Script)
Copyright (C) 2023 Pedram GANJEH HADIDI

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


namespace GeNSIS
{
    using GeNSIS.Core.Interfaces;
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

        public SettingsWindow(IConfig pAppConfig) : this()
        {
            Config = new ConfigVM(true);
            Config.UpdateValues(pAppConfig);
            Config.HasUnsavedChanges = false;
            DataContext = Config;
        }

        public ConfigVM Config { get; set; }

        private void OnSelectGeNsisProjectFolderClicked(object sender, RoutedEventArgs e)
        {
            if (m_FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Config.GeNSISProjectsDirectory = m_FBD.SelectedPath;
        }

        private void OnSelectScriptsFolderClicked(object sender, RoutedEventArgs e)
        {
            if (m_FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Config.ScriptsDirectory = m_FBD.SelectedPath;
        }

        private void OnSelectInstallersFolderClicked(object sender, RoutedEventArgs e)
        {
            if (m_FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Config.InstallersDirectory = m_FBD.SelectedPath;
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
