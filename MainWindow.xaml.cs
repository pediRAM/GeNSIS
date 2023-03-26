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
using GeNSIS.Core.Helpers;
using GeNSIS.Core.Models;
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

        private AppDataVM m_AppDataViewModel;
        private string m_ProjectName = "Unsaved";
        private OpenFileDialog m_OpenFileDialog = new OpenFileDialog();
        private OpenFileDialog m_OpenIconDialog = new OpenFileDialog();
        private SaveFileDialog m_SaveFileDialog = new SaveFileDialog();
        private FolderBrowserDialog m_FolderBrowserDialog = new FolderBrowserDialog();
        private ProjectManager m_ProjectManager = new ProjectManager();
        private MessageBoxManager m_MsgMgr = new MessageBoxManager();
        private AppConfig m_Config;
        

        private void NotifyPropertyChanged(string pPropertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));

        public MainWindow()
        {            
            AppData = new AppDataVM(true);            

            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Title = $"GeNSIS {AsmConst.VERSION}";
            editor.SyntaxHighlighting = XshdLoader.LoadHighlightingDefinitionOrNull("nsis.xshd");
            m_OpenFileDialog.Multiselect = true;
            m_OpenIconDialog.Filter = FileFilterHelper.GetIconFilter();
            m_SaveFileDialog.Filter = FileFilterHelper.GetNsisFilter() ;
            DataContext = AppData;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ProcessAppConfig();
        }

        private void ProcessAppConfig()
        {
            if (AppConfigHelper.AppConfigFileExists())
            {
                try
                {
                    m_Config = AppConfigHelper.ReadConfigFile();
                }
                catch(Exception ex)
                {
                    _ = m_MsgMgr.ShowLoadConfigError(ex);
                    Close();
                    return;
                }
            }
            else
            {
                m_Config = AppConfigHelper.CreateConfig();
                AppConfigHelper.WriteConfigFile(m_Config);
            }
        }

        public AppDataVM AppData
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
            {
                _ = m_MsgMgr.ShowInvalidDataError(error.ToString());
                return;
            }

            m_SaveFileDialog.Filter = FileFilterHelper.GetNsisFilter();
            m_SaveFileDialog.InitialDirectory = m_Config.ScriptsDirectory;
            if (m_SaveFileDialog.ShowDialog() != true)
                return;

            var g = new NsisGenerator();
            var nsisCode = g.Generate(AppData, new TextGeneratorOptions() { EnableComments = true, EnableLogs = true });
            SaveScript(m_SaveFileDialog.FileName, nsisCode);
            editor.Text = nsisCode;
            tabItem_Editor.IsSelected = true;
        }

        private void SaveScript(string pFileName, string pNsiString)
        {
            m_GeneratedNsisPath = pFileName;
            File.WriteAllText(pFileName, pNsiString, encoding: System.Text.Encoding.UTF8);            
        }

        private void OnCompileScript(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(m_GeneratedNsisPath))
            {
                _ = m_MsgMgr.ShowNoGeneratedScriptFileError();
                return;
            }

            if(!File.Exists(m_GeneratedNsisPath))
            {
                _ = m_MsgMgr.ShowGeneratedScriptFileNotFoundError();
                return;
            }

            if (string.IsNullOrEmpty(m_Config.NsisInstallationDirectory))
            {
                _ = m_MsgMgr.ShowSettingsHasNoNsisPathDefError();
                return;
            }
            var makeNsisExePath = $@"{m_Config.NsisInstallationDirectory}\makensisw.exe";
            var pi = new ProcessStartInfo($"\"{makeNsisExePath}\"", $"\"{m_GeneratedNsisPath}\"");
            pi.UseShellExecute = false;
            
            var proc = new Process();
            proc.StartInfo = pi;
            _ = proc.Start();
        }

        private void OnNewProjectClicked(object sender, RoutedEventArgs e)
        {
            if (AppData.HasUnsavedChanges)
            {
                if (m_MsgMgr.ShowUnsavedChangesByNewProjectWarning() != MessageBoxResult.Yes)
                    return;
            }

            AppData.UpdateValues(new AppData());
            ResetScriptAndPath();
        }

        private void ResetScriptAndPath()
        {
            editor.Clear();
            m_GeneratedNsisPath = null;
        }

        private void OnAddFilesFromFolderClicked(object sender, RoutedEventArgs e)
        {
            if (m_FolderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            foreach(var dir in Directory.GetDirectories(m_FolderBrowserDialog.SelectedPath, "*", SearchOption.TopDirectoryOnly)) 
                AppData.Directories.Add(dir);

            foreach (var file in Directory.GetFiles(m_FolderBrowserDialog.SelectedPath, "*", SearchOption.TopDirectoryOnly))
            {
                if (Path.GetExtension(file).Equals(".pdb", StringComparison.OrdinalIgnoreCase)) // todo: filter by ignore list!
                    continue;

                AppData.Files.Add(file);
                var ext = Path.GetExtension(file);
                if (!string.IsNullOrWhiteSpace(ext))
                {
                    switch (ext.ToLower())
                    {
                        case ".exe":
                            {
                                AppData.ExeName = file;
                                try
                                {
                                    ExeInfoHelper.AutoSetProperties(AppData);
                                }
                                catch { }
                            }
                            break;
                        case ".ico": AppData.AppIcon = file; break;

                        case ".rtf":
                        case ".txt":
                            {
                                var name = Path.GetFileName(file);
                                if (name.Contains("license", StringComparison.OrdinalIgnoreCase) ||
                                    name.Contains("eula", StringComparison.OrdinalIgnoreCase) ||
                                    name.Contains("agreement", StringComparison.OrdinalIgnoreCase))
                                    AppData.License = file;
                            }
                            break;
                    }
                }
            }

            ExeInfoHelper.AutoNameInstallerExe(AppData);
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            if (AppData.HasUnsavedChanges)
            {
                if (m_MsgMgr.ShowUnsavedChangesByClosingAppWarning() != MessageBoxResult.Yes)
                    return;
            }

            Close();
        }

        private void OnSaveProjectAsClicked(object sender, RoutedEventArgs e)
        {
            m_SaveFileDialog.Filter = FileFilterHelper.GetProjectFilter();
            m_SaveFileDialog.InitialDirectory = m_Config.GeNSISProjectsDirectory;
            if (m_SaveFileDialog.ShowDialog() != true)
                return;

            var project = new Project { AppData = AppData.ToModel() };
            m_ProjectManager.Save(m_SaveFileDialog.FileName, project);
            AppData.HasUnsavedChanges = false;
        }

        private void OnSaveProjectClicked(object sender, RoutedEventArgs e)
        {
            // Is project already saved (by "Save as...")?
            string pathOfSavedProject = m_ProjectManager.GetProjectFilePath();
            if (pathOfSavedProject == null)
            {
                // No! Project has not been saved yet.
                OnSaveProjectAsClicked(sender, e);
                return;
            }

            var project = new Project() { AppData = AppData.ToModel() };
            m_ProjectManager.Save(pathOfSavedProject, project);
            AppData.HasUnsavedChanges = false;
        }

        private void OnOpenProjectClicked(object sender, RoutedEventArgs e)
        {
            if(AppData.HasUnsavedChanges)
            {
                if (m_MsgMgr.ShowUnsavedChangesByOpenProjectWarning() != MessageBoxResult.Yes)
                    return;
            }

            m_OpenFileDialog.Filter = FileFilterHelper.GetProjectFilter();
            m_OpenFileDialog.InitialDirectory = m_Config.GeNSISProjectsDirectory;
            if (m_OpenFileDialog.ShowDialog() != true)
                return;

            var appData = m_ProjectManager.Load(m_OpenFileDialog.FileName).AppData;
            AppData.UpdateValues(appData);
            ResetScriptAndPath();
        }

        private void OnOpenSettingsWindowClicked(object sender, RoutedEventArgs e)
        {
            var sw = new SettingsWindow(m_Config);
            if (sw.ShowDialog() != true && sw.Config.HasUnsavedChanges)
            {
                if (m_MsgMgr.ShowSaveSettingChangesQuestion() != MessageBoxResult.Yes)
                    return;
            }

            if (sw.Config.HasUnsavedChanges)
            {
                m_Config.UpdateValues(sw.Config);
                AppConfigHelper.WriteConfigFile(m_Config);
            }
        }

        private void OnAboutClicked(object sender, RoutedEventArgs e)
            => OpenWebsiteInDefaultBrowser(@"https://github.com/pediRAM/GeNSIS/");

        private void OnManualClicked(object sender, RoutedEventArgs e)
            => OpenWebsiteInDefaultBrowser(@"https://github.com/pediRAM/GeNSIS/blob/main/README.md");

        private void OpenWebsiteInDefaultBrowser(string pUrl)
            => _ = Process.Start(new ProcessStartInfo(pUrl) { UseShellExecute = true });

    }
}
