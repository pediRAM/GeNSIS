﻿/***************************************************************************************
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
using GeNSIS.Core.Converters;
using GeNSIS.Core.Extensions;
using GeNSIS.Core.Helpers;
using GeNSIS.Core.Models;
using GeNSIS.Core.TextGenerators;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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

        #region Variables
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private AppConfig m_Config;
        private AppDataVM m_AppDataViewModel;

        private string m_ProjectName = "Unsaved";
        private string m_PathToGeneratedNsisScript;

        private OpenFileDialog m_OpenFilesDialog = new OpenFileDialog() { Multiselect = true };
        private OpenFileDialog m_OpenImageDialog = new OpenFileDialog();

        // BUG: .NET 6 seems to have some issues with setting value of
        // the property "InitialDirectory" -> so we try to create two
        // instances of SaveFileDialogs and using workaround in FileDialogHelper.
        private SaveFileDialog m_SaveScriptDialog = new SaveFileDialog();
        private SaveFileDialog m_SaveProjectDialog = new SaveFileDialog();

        private FolderBrowserDialog m_FolderBrowserDialog = new FolderBrowserDialog();
        private ProjectManager m_ProjectManager = new ProjectManager();

        private MessageBoxManager m_MsgBoxMgr = new MessageBoxManager();
        
        private NsisGenerator m_NsisCodeGenerator = new NsisGenerator();
        #endregion Variables


        #region Ctor
        public MainWindow()
        {            
            AppData = new AppDataVM(true);            

            InitializeComponent();
            Title = $"GeNSIS {AsmConst.VERSION}";
            editor.SyntaxHighlighting = XshdLoader.LoadHighlightingDefinitionOrNull("nsis.xshd");

            Loaded += OnMainWindowLoaded;            

            FileDialogHelper.InitDir(m_OpenFilesDialog, PathHelper.GetMyDocuments());
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.ICON;
            m_SaveScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;
            m_SaveProjectDialog.Filter = FileDialogHelper.Filter.PROJECT;

            DataContext = AppData;
        }
        #endregion Ctor


        #region Properties
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

        internal string PathToGeneratedNsisScript
        {
            get => m_PathToGeneratedNsisScript;
            set
            {
                if (value == m_PathToGeneratedNsisScript) return;
                m_PathToGeneratedNsisScript = value;
                NotifyPropertyChanged(nameof(PathToGeneratedNsisScript));
            }
        }
        #endregion Properties


        #region Methods
        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ProcessAppConfig();
            if (!NsisInstallationDirectoryExists())
            {
                if (m_MsgBoxMgr.ShowContinueWithoutNsisWarning() != MessageBoxResult.Yes)
                {
                    Log?.Debug("User decided to quit due to missing NSIS installation.");
                    Close();
                    return;
                }
            }
        }

        private bool NsisInstallationDirectoryExists()
        {
            if (string.IsNullOrEmpty(m_Config.NsisInstallationDirectory))
            {
                if (m_MsgBoxMgr.ShowDoYouWantToSelectNsisInstallDirManuallyQuestion() != MessageBoxResult.Yes)
                {
                    Log?.Debug("User denied manual nsis install dir selection!");
                    return false;
                }

                FileDialogHelper.InitDir(m_FolderBrowserDialog, PathHelper.GetProgramFilesX86NsisDir());
                if (m_FolderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    Log?.Debug("User canceled searching for nsis install dir!");
                    return false;
                }

                if (Directory.GetFiles(m_FolderBrowserDialog.SelectedPath, "*.exe").Any(f => f.EndsWith(" \\nsis.exe", StringComparison.OrdinalIgnoreCase)))
                {
                    Log?.Debug("NSIS install dir chosen by user seems to be ok.");
                    m_Config.NsisInstallationDirectory = m_FolderBrowserDialog.SelectedPath;

                    Log?.Debug("Saving changes to config file...");
                    AppConfigHelper.WriteConfigFile(m_Config);
                }
                else
                {
                    Log?.Warn("NSIS install dir chosen by user was not ok!");
                    return false;
                }
            }

            return true;
        }

        private void ProcessAppConfig()
        {
            if (AppConfigHelper.AppConfigFileExists())
            {
                try
                {
                    Log.Debug("Reading config file...");
                    m_Config = AppConfigHelper.ReadConfigFile();
                    Log.Debug("Reading config file suceeded.");

                    Log.Debug("Creating GeNSIS directories if not exist...");
                    AppConfigHelper.CreateGeNSISDirectoriesIfNotExist();
                    Log.Debug("Creating GeNSIS directories succeeded.");
                }
                catch(Exception ex)
                {
                    Log.Error(ex);
                    _ = m_MsgBoxMgr.ShowLoadConfigError(ex);
                    Close();
                    return;
                }
            }
            else
            {
                Log.Warn("Config file not found!");
                Log.Debug("Creating default configuration...");
                m_Config = AppConfigHelper.CreateConfig();
                Log.Info("Writing default config file...");
                AppConfigHelper.WriteConfigFile(m_Config);
                Log.Info("Writing default config file succeeded.");
            }
        }

        private void OnAddFilesClicked(object sender, RoutedEventArgs e)
        {
            FileDialogHelper.InitDir(m_OpenFilesDialog, PathHelper.GetMyDocuments());
            if (m_OpenFilesDialog.ShowDialog().Value != true) 
                return;

            AppData.Files.AddRange(m_OpenFilesDialog.FileNames);
        }

        private void OnLoadIconClicked(object sender, RoutedEventArgs e)
        {
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.ICON;
            FileDialogHelper.InitDir(m_OpenImageDialog, AppConfigHelper.GetNsisIconsFolder());
            if (m_OpenImageDialog.ShowDialog() == true)
            {
                try
                {
                    var fi = new FileInfo(m_OpenImageDialog.FileName);
                    if (fi.Extension.Equals(".ico", StringComparison.InvariantCultureIgnoreCase) && fi.Length > 0)
                    {
                        if (! AppData.Files.Contains(m_OpenImageDialog.FileName))
                            AppData.Files.Add(m_OpenImageDialog.FileName);
                    }
                    AppData.InstallerIcon = m_OpenImageDialog.FileName;
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
        
        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            var validator = new Core.Validator();
            ValidationError error = null;
            if (!validator.IsValid(AppData, out error))
            {
                _ = m_MsgBoxMgr.ShowInvalidDataError(error.ToString());
                return;
            }

            FileDialogHelper.InitDir(m_SaveScriptDialog, m_Config.ScriptsDirectory);
            m_SaveScriptDialog.FileName = PathHelper.GetNewScriptName(AppData);
            m_SaveScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;

            if (m_SaveScriptDialog.ShowDialog() != true)
                return;

            var nsisCode = m_NsisCodeGenerator.Generate(AppData, new TextGeneratorOptions() { EnableComments = true, EnableLogs = true });
            FileDialogHelper.InitDir(m_SaveScriptDialog, PathHelper.GetGeNSISScriptsDir());
            SaveScript(m_SaveScriptDialog.FileName, nsisCode);
            editor.Text = nsisCode;
            tabItem_Editor.IsSelected = true;
        }

        private void SaveScript(string pFileName, string pNsiString)
        {
            PathToGeneratedNsisScript = pFileName;
            File.WriteAllText(pFileName, pNsiString, encoding: System.Text.Encoding.UTF8);            
        }

        private void OnCompileScript(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(PathToGeneratedNsisScript))
            {
                _ = m_MsgBoxMgr.ShowNoGeneratedScriptFileError();
                return;
            }

            if(!File.Exists(PathToGeneratedNsisScript))
            {
                _ = m_MsgBoxMgr.ShowGeneratedScriptFileNotFoundError();
                return;
            }

            if (string.IsNullOrEmpty(m_Config.NsisInstallationDirectory))
            {
                _ = m_MsgBoxMgr.ShowSettingsHasNoNsisPathDefError();
                return;
            }
            var makeNsisExePath = $@"{m_Config.NsisInstallationDirectory}\makensisw.exe";
            var pi = new ProcessStartInfo($"\"{makeNsisExePath}\"", $"/V4 /NOCD \"{PathToGeneratedNsisScript}\"");
            pi.UseShellExecute = false;
            
            var proc = new Process();
            proc.StartInfo = pi;
            _ = proc.Start();
        }

        private void OnNewProjectClicked(object sender, RoutedEventArgs e)
        {
            if (AppData.HasUnsavedChanges)
            {
                if (m_MsgBoxMgr.ShowUnsavedChangesByNewProjectWarning() != MessageBoxResult.Yes)
                    return;
            }

            AppData.UpdateValues(new AppData());
            ResetScriptAndPath();
        }

        private void ResetScriptAndPath()
        {
            editor.Clear();
            PathToGeneratedNsisScript = null;
        }

        private void OnAddFilesFromFolderClicked(object sender, RoutedEventArgs e)
        {
            FileDialogHelper.InitDir(m_FolderBrowserDialog, PathHelper.GetMyDocuments());
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
                        case ".ico": AppData.InstallerIcon = file; break;

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
                if (m_MsgBoxMgr.ShowUnsavedChangesByClosingAppWarning() != MessageBoxResult.Yes)
                    return;
            }

            Close();
        }

        private void OnSaveProjectAsClicked(object sender, RoutedEventArgs e)
        {
            m_SaveProjectDialog.Filter = FileDialogHelper.Filter.PROJECT;
            FileDialogHelper.InitDir(m_SaveProjectDialog, m_Config.GeNSISProjectsDirectory);
            m_SaveProjectDialog.FileName = PathHelper.GetNewProjectName(AppData);
            if (m_SaveProjectDialog.ShowDialog() != true)
                return;

            var project = new Project { AppData = AppData.ToModel() };
            m_ProjectManager.Save(m_SaveProjectDialog.FileName, project);
            AppData.ResetHasUnsavedChanges();
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
            AppData.ResetHasUnsavedChanges();
        }

        private void OnOpenProjectClicked(object sender, RoutedEventArgs e)
        {
            if(AppData.HasUnsavedChanges)
            {
                if (m_MsgBoxMgr.ShowUnsavedChangesByOpenProjectWarning() != MessageBoxResult.Yes)
                    return;
            }

            m_OpenFilesDialog.Filter = FileDialogHelper.Filter.PROJECT;
            m_OpenFilesDialog.InitialDirectory = m_Config.GeNSISProjectsDirectory;
            if (m_OpenFilesDialog.ShowDialog() != true)
                return;

            var appData = m_ProjectManager.Load(m_OpenFilesDialog.FileName).AppData;
            AppData.UpdateValues(appData);
            ResetScriptAndPath();
        }

        private void OnOpenSettingsWindowClicked(object sender, RoutedEventArgs e)
        {
            var sw = new SettingsWindow(m_Config);
            if (sw.ShowDialog() != true && sw.Config.HasUnsavedChanges)
            {
                if (m_MsgBoxMgr.ShowSaveSettingChangesQuestion() != MessageBoxResult.Yes)
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

        private void OnLoadWizardClicked(object sender, RoutedEventArgs e)
        {
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.BITMAP;
            m_OpenImageDialog.InitialDirectory = AppConfigHelper.GetNsisWizardImagesFolder();
            if (m_OpenImageDialog.ShowDialog() == true)
            {
                try
                {
                    var fi = new FileInfo(m_OpenImageDialog.FileName);
                    if (fi.Extension.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase) && fi.Length > 0)
                    {
                        Image image = Image.FromFile(m_OpenImageDialog.FileName);
                        if (image.Width != 164 || image.Height != 314)
                        {
                            _ = m_MsgBoxMgr.ShowWizardImageBadSizeWarn();
                            return;
                        }
                    }
                    AppData.InstallerWizardImage = m_OpenImageDialog.FileName;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
            }
        }

        private void OnLoadHeaderClicked(object sender, RoutedEventArgs e)
        {
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.BITMAP;
            m_OpenImageDialog.InitialDirectory = AppConfigHelper.GetNsisHeaderImagesFolder();
            if (m_OpenImageDialog.ShowDialog() == true)
            {
                try
                {
                    var fi = new FileInfo(m_OpenImageDialog.FileName);
                    if (fi.Extension.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase) && fi.Length > 0)
                    {
                        Image image = Image.FromFile(m_OpenImageDialog.FileName);
                        if (image.Width != 150 || image.Height != 57)
                        {
                            _ = m_MsgBoxMgr.ShowBannerImageBadSizeWarn();
                            return;
                        }
                    }
                    AppData.InstallerHeaderImage = m_OpenImageDialog.FileName;
                    var conv = new StringToImageSourceConverter();
                    img_License.Source = (System.Windows.Media.ImageSource)conv.Convert(@"Resources\Images\Installer\Custom\license.png");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
            }
        }

        private void OnSaveScriptClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathToGeneratedNsisScript))
            {
                _ = m_MsgBoxMgr.ShowNoGeneratedScriptFileError();
                return; 
            }

            File.WriteAllText(PathToGeneratedNsisScript, editor.Text, System.Text.Encoding.UTF8);
            _ = m_MsgBoxMgr.ShowSavingScriptSucceededInfo();
        }
        #endregion Methods
    }
}
