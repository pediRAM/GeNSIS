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


using GeNSIS.Core;
using GeNSIS.Core.Converters;
using GeNSIS.Core.Extensions;
using GeNSIS.Core.Helpers;
using GeNSIS.Core.Interfaces;
using GeNSIS.Core.Models;
using GeNSIS.Core.TextGenerators;
using GeNSIS.Core.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using ICSharpCode.AvalonEdit.Utils;
using System.Windows.Documents;
using ExtendedXmlSerializer;
using System.Windows.Input;

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
        private SingletonStorage m_Storage = new SingletonStorage();
        private Config m_Config;
        private AppDataVM m_AppDataViewModel;

        private string m_ProjectName = "Unsaved";
        private string m_PathToGeneratedNsisScript;

        private OpenFileDialog m_OpenFilesDialog = new OpenFileDialog() { Multiselect = true };
        private OpenFileDialog m_OpenScriptDialog = new OpenFileDialog();
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
            m_Storage.Put<IAppData>(AppData);

            InitializeComponent();
            Loaded += OnMainWindowLoaded;

            Title = $"GeNSIS {AsmConst.FULL_VERSION}";
            editor.SyntaxHighlighting = XshdLoader.LoadHighlightingDefinitionOrNull("nsis.xshd");

            FileDialogHelper.InitDir(m_OpenFilesDialog, PathHelper.GetMyDocuments());
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.ICON;
            m_SaveScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;
            m_SaveProjectDialog.Filter = FileDialogHelper.Filter.PROJECT;

            InitLanguages();
            m_Storage.Put<ObservableCollection<Language>>(LangDst);
            DataContext = AppData;
        }

        private void CheckAndLoadProjectPathFromArguments()
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args != null && args.Length >= 2 && File.Exists(args[1]))
                {
                    Log.Debug($"Found existing path in args:'{args[1]}'");
                    LoadProject(args[1]);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void InitLanguages()
        {
            var languages = LanguageHelper.GetLanguages();

            foreach (var name in LanguageHelper.GetNamesOfMostSpockenLanguages())
            {
                var lang = languages.Single(l => l.Name == name);
                languages.Remove(lang);
                LangDst.Add(lang);
            }

            LangSrc.AddRange(languages);
            lsb_LangSrc.ItemsSource = LangSrc;
            lsb_LangDst.ItemsSource = LangDst;
        }
        #endregion Ctor


        #region Properties
        public ObservableCollection<Language> LangSrc { get; set; } = new ObservableCollection<Language>();
        public ObservableCollection<Language> LangDst { get; set; } = new ObservableCollection<Language>();
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
            Log.Info("Main window has loaded.");

            ProcessAppConfig();
            if (!NsisInstallationDirectoryExists())
            {
                if (m_MsgBoxMgr.ShowContinueWithoutNsisWarning() != MessageBoxResult.Yes)
                {
                    Log.Debug("User decided to quit due to missing NSIS installation.");
                    Close();
                    return;
                }
            }

            CheckAndLoadProjectPathFromArguments();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (AppData.HasUnsavedChanges)
            {
                if (m_MsgBoxMgr.ShowUnsavedChangesByClosingAppWarning() != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnClosing(e);
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
                    ConfigHelper.WriteConfigFile(m_Config);
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
            if (ConfigHelper.AppConfigFileExists())
            {
                try
                {
                    Log.Debug("Reading config file...");
                    m_Config = ConfigHelper.ReadConfigFile();
                    Log.Debug("Reading config file suceeded.");

                    Log.Debug("Creating GeNSIS directories if not exist...");
                    ConfigHelper.CreateGeNSISDirectoriesIfNotExist();
                    Log.Debug("Creating GeNSIS directories succeeded.");
                }
                catch (Exception ex)
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
                m_Config = ConfigHelper.CreateConfig();
                Log.Info("Writing default config file...");
                ConfigHelper.WriteConfigFile(m_Config);
                Log.Info("Writing default config file succeeded.");
            }
        }

        private void OnAddFilesClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked on AddFiles.");

            m_OpenFilesDialog.Filter = FileDialogHelper.Filter.ALL_FILES;
            FileDialogHelper.InitDir(m_OpenFilesDialog, PathHelper.GetMyDocuments());
            if (m_OpenFilesDialog.ShowDialog().Value != true)
                return;

            AppData.Files.AddRange(FileSystemItemVM.From(m_OpenFilesDialog.FileNames));
        }


        private void OnAddDirectoryClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked on AddDirectory.");
            if (m_FolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AppData.Sections.Add(new SectionVM(m_FolderBrowserDialog.SelectedPath));
            }
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked on OnGenerate.");
            try
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

                var nsisCode = m_NsisCodeGenerator.Generate(AppData, new TextGeneratorOptions() { EnableComments = true, EnableLogs = true, Languages = LangDst.ToList() });
                FileDialogHelper.InitDir(m_SaveScriptDialog, PathHelper.GetGeNSISScriptsDir());
                SaveScript(m_SaveScriptDialog.FileName, nsisCode);
                editor.Text = nsisCode;
                tabItem_Editor.IsSelected = true;
            }
            catch (FileNotFoundException fnfEx)
            {
                Log.Error(fnfEx);
                m_MsgBoxMgr.ShowContentFileNotFoundError(fnfEx.FileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                m_MsgBoxMgr.ShowException(ex);
            }
        }

        private void SaveScript(string pFileName, string pNsiString)
        {
            Log.Debug($"Saving nsis script to file:'{pFileName}' ...");
            PathToGeneratedNsisScript = pFileName;
            File.WriteAllText(pFileName, pNsiString, encoding: Encoding.UTF8);
        }

        private void OnCompileScript(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked on CompileScript.");
            if (string.IsNullOrEmpty(PathToGeneratedNsisScript))
            {
                Log.Warn($"{nameof(PathToGeneratedNsisScript)} was null or empty!");
                _ = m_MsgBoxMgr.ShowNoGeneratedScriptFileError();
                return;
            }

            if (!File.Exists(PathToGeneratedNsisScript))
            {
                Log.Warn($"File not found in:'{PathToGeneratedNsisScript}'!");
                _ = m_MsgBoxMgr.ShowGeneratedScriptFileNotFoundError();
                return;
            }

            if (string.IsNullOrEmpty(m_Config.NsisInstallationDirectory))
            {
                Log.Warn($"{nameof(m_Config.NsisInstallationDirectory)} was null or empty!");
                _ = m_MsgBoxMgr.ShowSettingsHasNoNsisPathDefError();
                return;
            }

            StartMakensiswProcess();
        }

        private void StartMakensiswProcess()
        {
            Log.Info("Starting makensisw process...");
            Directory.SetCurrentDirectory(m_Config.InstallersDirectory);
            var makeNsisExePath = $@"{m_Config.NsisInstallationDirectory}\makensisw.exe";
            var pi = new ProcessStartInfo($"\"{makeNsisExePath}\"", $"/V4 /NOCD \"{PathToGeneratedNsisScript}\"");
            pi.UseShellExecute = false;

            var proc = new Process();
            proc.StartInfo = pi;
            _ = proc.Start();
        }

        private void OnNewProjectClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked NewProject.");
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
            Log.Debug("Resetting script and path...");
            editor.Clear();
            PathToGeneratedNsisScript = null;
        }

        private void OnAddFilesFromFolderClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked AddFilesFromFolder.");
#if DEBUG
            m_FolderBrowserDialog.InitialDirectory = @"H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3"; // todo: <-- remove!
#else
            FileDialogHelper.InitDir(m_FolderBrowserDialog, PathHelper.GetMyDocuments());
#endif
            if (m_FolderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            AppData.AddFiles(Directory.GetDirectories(m_FolderBrowserDialog.SelectedPath, "*", SearchOption.TopDirectoryOnly));
            AppData.AddFiles(Directory.GetFiles(m_FolderBrowserDialog.SelectedPath, "*", SearchOption.TopDirectoryOnly));
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked Close.");
            Close();
        }

        private void OnSaveProjectAsClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SaveProjectAs.");
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
            Log.Info("User clicked SaveProject.");
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
            Log.Info("User clicked OpenProject.");
            if (AppData.HasUnsavedChanges)
            {
                if (m_MsgBoxMgr.ShowUnsavedChangesByOpenProjectWarning() != MessageBoxResult.Yes)
                    return;
            }

            m_OpenFilesDialog.Filter = FileDialogHelper.Filter.PROJECT;
            m_OpenFilesDialog.InitialDirectory = m_Config.GeNSISProjectsDirectory;
            if (m_OpenFilesDialog.ShowDialog() != true)
                return;

            LoadProject(m_OpenFilesDialog.FileName);
        }

        private void LoadProject(string pPathToProjectFile)
        {
            Log.Debug($"Loading project file from:'{pPathToProjectFile}'");

            var appData = m_ProjectManager.Load(pPathToProjectFile).AppData;
            AppData.UpdateValues(appData);
            ResetScriptAndPath();
        }

        private void OnOpenSettingsWindowClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked OpenSettingsWindow.");
            var sw = new SettingsWindow(m_Config);
            sw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (sw.ShowDialog() != true && sw.Config.HasUnsavedChanges)
            {
                if (m_MsgBoxMgr.ShowSaveSettingChangesQuestion() != MessageBoxResult.Yes)
                    return;
            }

            if (sw.Config.HasUnsavedChanges)
            {
                m_Config.UpdateValues(sw.Config);
                ConfigHelper.WriteConfigFile(m_Config);
            }
        }

        private void OnAboutClicked(object sender, RoutedEventArgs e)
            => OpenWebsiteInDefaultBrowser(@"https://github.com/pediRAM/GeNSIS/");

        private void OnManualClicked(object sender, RoutedEventArgs e)
            => OpenWebsiteInDefaultBrowser(@"https://github.com/pediRAM/GeNSIS/blob/main/README.md");

        private void OpenWebsiteInDefaultBrowser(string pUrl)
            => _ = Process.Start(new ProcessStartInfo(pUrl) { UseShellExecute = true });

        #region Open/Save Script
        private void OnSaveScriptClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SaveScript.");
            if (string.IsNullOrWhiteSpace(PathToGeneratedNsisScript))
            {
                _ = m_MsgBoxMgr.ShowNoGeneratedScriptFileError();
                return;
            }

            File.WriteAllText(PathToGeneratedNsisScript, editor.Text, System.Text.Encoding.UTF8);
            _ = m_MsgBoxMgr.ShowSavingScriptSucceededInfo();
        }
        #endregion Methods

        private void OnOpenScriptClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked OpenScript.");
            m_OpenScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;
            FileDialogHelper.InitDir(m_OpenScriptDialog, PathHelper.GetGeNSISScriptsDir());
            if (m_OpenScriptDialog.ShowDialog() != true)
                return;

            LoadScriptFromFile(m_OpenScriptDialog.FileName);
        }

        private void LoadScriptFromFile(string pScriptPath)
        {
            Log.Debug($"Loading script from file:'{pScriptPath}' ...");
            try
            {
                editor.Text = File.ReadAllText(pScriptPath, Encoding.UTF8);
                tabItem_Editor.IsSelected = true;
                PathToGeneratedNsisScript = pScriptPath;
            }
            catch (FileNotFoundException fnfEx)
            {
                Log.Error(fnfEx);
                m_MsgBoxMgr.ShowScriptNotFoundError(fnfEx.FileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                m_MsgBoxMgr.ShowException(ex);
            }
        }
        #endregion Open/Save Script

        #region Desing (Icons & Images)
        private void OnLoadIconClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked LoadIcon.");
            if (TryLoadIcon(out string fileName))
                AppData.InstallerIcon = fileName;
        }

        private void OnLoadUninstallerIconClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked LoadUninstallerIcon.");
            if (TryLoadIcon(out string fileName))
                AppData.UninstallerIcon = fileName;
        }

        private bool TryLoadIcon(out string pFileName)
        {
            pFileName = null;
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.ICON;
            FileDialogHelper.InitDir(m_OpenImageDialog, ConfigHelper.GetNsisIconsFolder());
            if (m_OpenImageDialog.ShowDialog() == true)
            {
                try
                {
                    var fi = new FileInfo(m_OpenImageDialog.FileName);
                    if (fi.Extension.Equals(".ico", StringComparison.InvariantCultureIgnoreCase) && fi.Length > 0)
                    {
                        if (!AppData.Files.Any(x => x.Path.Equals(m_OpenImageDialog.FileName, StringComparison.OrdinalIgnoreCase)))
                            AppData.Files.Add(new FileSystemItemVM(m_OpenImageDialog.FileName));
                    }
                    pFileName = m_OpenImageDialog.FileName;
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            return false;
        }

        private void OnLoadWizardClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked LoadWizard.");
            if (TryLoadWizardImage(out string fileName))
                AppData.InstallerWizardImage = fileName;
        }

        private void OnLoadUninstallerWizardClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked LoadUninstallerWizard.");
            if (TryLoadWizardImage(out string fileName))
                AppData.UninstallerWizardImage = fileName;
        }

        private bool TryLoadWizardImage(out string pFileName)
        {
            pFileName = null;
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.BITMAP;
            m_OpenImageDialog.InitialDirectory = ConfigHelper.GetNsisWizardImagesFolder();
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
                            return false;
                        }
                    }
                    pFileName = m_OpenImageDialog.FileName;
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            return false;
        }

        private void OnLoadHeaderClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked LoadHeader.");
            if (TryLoadHeaderImage(out string fileName))
                AppData.InstallerHeaderImage = fileName;
        }

        private void OnLoadUninstallerHeaderClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked LoadUninstallerHeader.");
            if (TryLoadHeaderImage(out string fileName))
                AppData.UninstallerHeaderImage = fileName;
        }

        private bool TryLoadHeaderImage(out string pFileName)
        {
            pFileName = null;
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.BITMAP;
            m_OpenImageDialog.InitialDirectory = ConfigHelper.GetNsisHeaderImagesFolder();
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
                            return false;
                        }
                    }
                    pFileName = m_OpenImageDialog.FileName;
                    var conv = new StringToImageSourceConverter();
                    img_License.Source = (System.Windows.Media.ImageSource)conv.Convert(@"Resources\Images\Installer\Custom\license.png");
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            return false;
        }

        #endregion Desing (Icons & Images)

        #region Sorting Languages
        private bool m_SortByName;
        private void OnSortByNameClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SortByName.");
            if (!m_SortByName)
            {
                var l = LangSrc.ToList();
                LangSrc.Clear();
                l = l.OrderBy(x => x.Name).ToList();
                LangSrc.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "Z→A";
            }
            else
            {
                var l = LangSrc.ToList();
                LangSrc.Clear();
                l = l.OrderByDescending(x => x.Name).ToList();
                LangSrc.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "A→Z";
            }

            m_SortByName = !m_SortByName;
        }

        private bool m_SortByOrder;
        private void OnSortByOrderClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SortByOrder.");
            if (!m_SortByOrder)
            {
                var l = LangSrc.ToList();
                LangSrc.Clear();
                l = l.OrderBy(x => x.Order).ToList();
                LangSrc.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "N→1";
            }
            else
            {
                var l = LangSrc.ToList();
                LangSrc.Clear();
                l = l.OrderByDescending(x => x.Order).ToList();
                LangSrc.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "1→N";
            }

            m_SortByOrder = !m_SortByOrder;
        }
        #endregion Sorting Languages

        #region Sorting Languages
        private bool m_SortDstByName;
        private void OnSortDstByNameClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SortDstByName.");
            if (!m_SortDstByName)
            {
                var l = LangDst.ToList();
                LangDst.Clear();
                l = l.OrderBy(x => x.Name).ToList();
                LangDst.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "Z→A";
            }
            else
            {
                var l = LangDst.ToList();
                LangDst.Clear();
                l = l.OrderByDescending(x => x.Name).ToList();
                LangDst.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "A→Z";
            }

            m_SortDstByName = !m_SortByName;
        }

        private bool m_SortDstByOrder;
        private void OnSortDstByOrderClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SortDstByOrder.");
            if (!m_SortDstByOrder)
            {
                var l = LangDst.ToList();
                LangDst.Clear();
                l = l.OrderBy(x => x.Order).ToList();
                LangDst.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "N→1";
            }
            else
            {
                var l = LangDst.ToList();
                LangDst.Clear();
                l = l.OrderByDescending(x => x.Order).ToList();
                LangDst.AddRange(l);
                (sender as System.Windows.Controls.Button).Content = "1→N";
            }

            m_SortDstByOrder = !m_SortDstByOrder;
        }
        #endregion Sorting Languages
        private void OnListBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var c = e.Key.ToString();
            var elem = LangSrc.FirstOrDefault(x => x.Name.StartsWith(c, StringComparison.OrdinalIgnoreCase));
            if (elem != null)
                lsb_LangSrc.ScrollIntoView(elem);
        }

        private void OnAddSelectedLanguagesClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked AddSelectedLanguages.");
            var selectedLanguages = lsb_LangSrc.GetSelectedItems<Language>();
            LangSrc.RemoveRange(selectedLanguages);
            LangDst.AddRange(selectedLanguages);
        }

        private void OnAddAllLanguagesClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked AddAllLanguages.");
            LangDst.AddRange(LangSrc);
            LangSrc.Clear();
        }

        private void OnRemoveSelectedLanguagesClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked RemoveSelectedLanguages.");
            var selectedLanguages = lsb_LangDst.GetSelectedItems<Language>();
            LangDst.RemoveRange(selectedLanguages);
            LangSrc.AddRange(selectedLanguages);
        }

        private void OnResetLanguagesClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked ResetLanguages.");
            LangSrc.AddRange(LangDst);
            LangDst.Clear();
            var eng = LangSrc.Single(x => x.Name == "English");
            LangSrc.Remove(eng);
            LangDst.Add(eng);
        }

        private void OnMoveSelectedLanguagesToBeginClicked(object sender, RoutedEventArgs e)
            => ReorderSelectedLanguages(LangDst.MoveFirst);

        private void OnMoveSelectedLanguagesToEndClicked(object sender, RoutedEventArgs e)
            => ReorderSelectedLanguages(LangDst.MoveLast);

        private void OnMoveSelectedLanguagesToPrevClicked(object sender, RoutedEventArgs e)
            => ReorderSelectedLanguages(LangDst.MovePrev);

        private void OnMoveSelectedLanguagesToNextClicked(object sender, RoutedEventArgs e)
            => ReorderSelectedLanguages(LangDst.MoveNext);

        private void ReorderSelectedLanguages(Action<List<Language>> pAction)
        {
            if (lsb_LangDst.SelectedItems == null || lsb_LangDst.SelectedItems.Count == 0)
                return;
            var items = lsb_LangDst.GetSelectedItems<Language>();
            pAction(items);
        }

        private void OnRemoveEmptyLinesAndCommentsClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked RemoveEmptyLinesAndComments.");
            var sb = new StringBuilder();
            foreach (string line in editor.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith(";")) continue;
                sb.AppendLine(line);
            }
            Dispatcher.Invoke(() => editor.Text = sb.ToString());
        }

        private void OnRemoveSelectedFilesClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked RemoveSelectedFiels.");
            if (lsb_Files.SelectedItems == null || lsb_Files.SelectedItems.Count == 0)
                return;

            var items = lsb_Files.GetSelectedItems<FileSystemItemVM>();
            AppData.Files.RemoveRange(items);
        }

        private void OnClearFilesClicked(object sender, RoutedEventArgs e)
            => AppData.Files.Clear();

        private void OnDirectoryDroped(object sender, DragEventArgs e)
        {
            Log.Info("User has dropped directory.");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                IEnumerable<string> files = e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string>;
                if (files != null)
                    AppData.AddFiles(files);
            }
        }

        private void OnScriptDropped(object sender, DragEventArgs e)
        {
            Log.Info("User dropped nsis script.");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (string path in paths)
                {
                    if (Path.GetExtension(paths[0]).Equals(".nsi", StringComparison.InvariantCultureIgnoreCase))
                    {
                        LoadScriptFromFile(path);
                        return;
                    }
                }
            }
        }

        private void OnPrintScriptClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked PrintScript.");
            var printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var rtfBox = new System.Windows.Controls.RichTextBox();
                rtfBox.Document = DocumentPrinter.CreateFlowDocumentForEditor(editor);
                printDialog.PrintDocument((((IDocumentPaginatorSource)rtfBox).DocumentPaginator), "printing as paginator");
            }
        }

        private void OnFilesKeyDown(object sender, KeyEventArgs e)
        {
            Log.Info($"Files: User pressed Key:{e.Key}.");
            if (lsb_Files.SelectedItems.Count == 0) return;

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) && (e.Key == Key.A)))
                lsb_Files.SelectAll();
            if (e.Key == Key.Subtract || e.Key == Key.OemMinus || e.Key == Key.Delete)
                AppData.Files.RemoveRange(lsb_Files.GetSelectedItems<FileSystemItemVM>());
        }

        private void OnFirewallRulesKeyDown(object sender, KeyEventArgs e)
        {
            Log.Info($"FirewallRules: User pressed Key:{e.Key}.");
            if (lsb_FirewallRules.SelectedItems.Count == 0) return;

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) && (e.Key == Key.A)))
                lsb_FirewallRules.SelectAll();
            if (e.Key == Key.Subtract || e.Key == Key.OemMinus || e.Key == Key.Delete)
                AppData.FirewallRules.RemoveRange(lsb_FirewallRules.GetSelectedItems<FirewallRuleVM>());
        }
    }

}
