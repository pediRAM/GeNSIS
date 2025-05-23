﻿/*
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


using ExtendedXmlSerializer;
using GeNSIS.Core;
using GeNSIS.Core.Converters;
using GeNSIS.Core.Enums;
using GeNSIS.Core.Extensions;
using GeNSIS.Core.Helpers;
using GeNSIS.Core.Interfaces;
using GeNSIS.Core.Managers;
using GeNSIS.Core.Models;
using GeNSIS.Core.Models.Design;
using GeNSIS.Core.TextGenerators;
using GeNSIS.Core.ViewModels;
using GeNSIS.UI;
using GeNSIS.UI.CustomDialogs;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;
using LiteDB;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using Image = System.Drawing.Image;
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
        //private SaveFileDialog m_SaveProjectDialog = new SaveFileDialog();

        private FolderBrowserDialog m_FolderBrowserDialog = new FolderBrowserDialog();

        private LiteDatabase m_liteDatabase;
        private ProjectManager m_ProjectManager;
        private DesignManager m_DesignManager;
        private LanguageGroupManager m_LanguageGroupManager;

        private MessageBoxManager m_MsgBoxMgr = new MessageBoxManager();

        private NsisGenerator m_NsisCodeGenerator = new NsisGenerator();

        private FileHistoryVM m_ProjectHistory;
        private FileHistoryVM m_ScriptHistory;
        #endregion Variables

        #region Ctor
        public MainWindow()
        {
            m_Config = IocContainer.Instance.Get<Config>();
            AppData = new AppDataVM(true);
            IocContainer.Instance.Put<IAppData>(AppData);

            InitializeComponent();
            Loaded += OnMainWindowLoaded;

            m_liteDatabase   = new LiteDatabase($"{PathHelper.GetInstallerssDir()}\\GeNSIS.db");
            /*
            var collections = m_liteDatabase.GetCollectionNames();
            foreach (var collection in collections)
                m_liteDatabase.DropCollection(collection);
            */
            m_ProjectManager = new ProjectManager(m_liteDatabase, "Projects");
            m_DesignManager  = new DesignManager(m_liteDatabase, "Designs");
            m_LanguageGroupManager = new LanguageGroupManager(m_liteDatabase, "LanguageGroups");

            Title = $"GeNSIS {AsmConst.FULL_VERSION}";
            editor.SyntaxHighlighting = XshdLoader.LoadHighlightingDefinitionOrNull("nsis.xshd");
            editor.TextArea.TextView.Margin = new Thickness(5, 0, 0, 0);

            FileDialogHelper.InitDir(m_OpenFilesDialog, PathHelper.GetMyDocuments());
            m_OpenImageDialog.Filter = FileDialogHelper.Filter.ICON;
            m_SaveScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;
            //m_SaveProjectDialog.Filter = FileDialogHelper.Filter.PROJECT;

            InitLanguages();
            // todo: InitForms();
            lsb_Forms.DataContext = Forms;
            IocContainer.Instance.Put<ObservableCollection<Language>>(LangDst);
            DataContext = AppData;

            _ = InitFileHistoriesAsync();
        }

        private async Task InitFileHistoriesAsync()
        {
            string dir = ConfigHelper.GetInstallationFolder();
            var projects = $"{dir}\\ProjectsHistory.json";
            var scripts = $"{dir}\\ScriptstsHistory.json";

            m_ProjectHistory = new FileHistoryVM(projects);
            m_ScriptHistory = new FileHistoryVM(scripts);
            

            await Task.Run(() => m_ProjectHistory.Load());
            await Task.Run(() => m_ScriptHistory.Load());

            cbx_LastProjects.DataContext = m_ProjectHistory;
            cbx_LastScripts.DataContext = m_ScriptHistory;
        }

        private void CheckAndLoadProjectPathFromArguments()
        {
            try
            {
                // todo: refactor for drag'n draw of exported projects/scripts!
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

        // todo: implement!
        private void InitForms()
        {
            throw new NotImplementedException();
        }
        #endregion Ctor


        #region Properties
        public ObservableCollection<Language> LangSrc { get; set; } = new ObservableCollection<Language>();
        public ObservableCollection<Language> LangDst { get; set; } = new ObservableCollection<Language>();

        public ObservableCollection<SettingGroup> Forms { get; set; } = new ObservableCollection<SettingGroup>();

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
            // todo: refactor for file exports! CheckAndLoadProjectPathFromArguments();
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
                _ = Dispatcher.BeginInvoke(() => AppData.Sections.Add(new SectionVM(m_FolderBrowserDialog.SelectedPath)));
            }
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked on OnGenerate.");
            var res = m_MsgBoxMgr.ShowQuestion("User confirmation needed", "Generating script will overwrite your script and throw away changes!\nAre you sure you want to generate script (again)?");
            if (res != MessageBoxResult.Yes)
                return;
            Dispatcher.Invoke(() => GenerateScript());
        }

        private void GenerateScript()
        {
            try
            {
                if (!IsInputDataValid(out ValidationData error))
                {
                    _ = m_MsgBoxMgr.ShowInvalidDataError(error.ToString());
                    return;
                }

                FileDialogHelper.InitDir(m_SaveScriptDialog, m_Config.ScriptsDirectory);
                m_SaveScriptDialog.FileName = PathHelper.GetNewScriptName(AppData);
                m_SaveScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;

                if (m_SaveScriptDialog.ShowDialog() != true)
                    return;

                
#if DEBUG
                // todo: REMOVE!
                var fg = new CustomPageGenerator();
                var settingGroups = fg.GetSettingGroups();
#else
                List<SettingGroup> settingGroups = new();
#endif
                var nsisCode = m_NsisCodeGenerator.Generate(AppData, new TextGeneratorOptions() { EnableComments = true, EnableLogs = true, Languages = LangDst.ToList(), SettingGroups = settingGroups });
                FileDialogHelper.InitDir(m_SaveScriptDialog, PathHelper.GetScriptsDir());
                SaveScript(m_SaveScriptDialog.FileName, nsisCode);
                editor.Text = nsisCode;
                tabItem_Editor.IsSelected = true;

                AddAndSaveLastScript(m_SaveScriptDialog.FileName);
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

        private bool IsInputDataValid(out ValidationData pError)
        {
            var validator = new Validator();
            pError = null;
            return validator.IsValid(AppData, out pError);
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
            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(m_Config.InstallersDirectory);
            var makeNsisExePath = $@"{m_Config.NsisInstallationDirectory}\makensisw.exe";
            var pi = new ProcessStartInfo($"\"{makeNsisExePath}\"", $"/V4 /NOCD \"{PathToGeneratedNsisScript}\"");
            pi.UseShellExecute = false;
            var proc = new Process();
            proc.StartInfo = pi;
            _ = proc.Start();
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void OpenScriptInExternalEditor()
        {
            Log.Info("Opening script in external editor...");
            string extEditorPath = string.IsNullOrWhiteSpace(m_Config.ExternalEditor) ? GConst.Default.EXTERNAL_EDITOR : m_Config.ExternalEditor;

            var pi = new ProcessStartInfo($"\"{extEditorPath}\"", $"\"{PathToGeneratedNsisScript}\"");
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

        private AppData CreateNewProject()
        {
            return new AppData
            {
                Publisher = m_Config.Publisher,
                Url = m_Config.Website,
                Company = m_Config.CompanyName
            };
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
            m_FolderBrowserDialog.InitialDirectory = @"C:\Users\pedra\source\repos\MyApp\Source\bin\Debug"; // todo: <-- remove!
#else
            FileDialogHelper.InitDir(m_FolderBrowserDialog, PathHelper.GetMyDocuments());
#endif
            if (m_FolderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            AddDirectoryContent(m_FolderBrowserDialog.SelectedPath, "*", SearchOption.TopDirectoryOnly);
        }

        private void AddDirectoryContent(string pDirPath, string pPattern = "*", SearchOption pSearchOption = SearchOption.TopDirectoryOnly)
        {
            AppData.AddFiles(Directory.GetDirectories(pDirPath, pPattern, pSearchOption));
            AppData.AddFiles(Directory.GetFiles(pDirPath, pPattern, pSearchOption));
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked Close.");
            Close();
        }

        private void OnSaveProjectAsClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SaveProjectAs.");
            // fix: invalid cast exception!
            AddNewProject(); // todo: fix Invalid Cast Exception!
        }

        private void AddNewProject()
        {
            var dlg = CustomDialogHelper.CreateFilenameInputDialog("Name of Project", "Project Name", "Please type a unique project name:");
            if (dlg.ShowDialog() != true)
                return;

            if (string.IsNullOrWhiteSpace(dlg.Text))
            {
                m_MsgBoxMgr.ShowError("Empty/Illegal project name!", "Given project name is empty or contains illegal characters!");
                return;
            }

            if (m_ProjectManager.Items.Any(x => x.Name == dlg.Name))
            {
                m_MsgBoxMgr.ShowError("Project name already in use!", "Given project name is already in use!\nPlease repeat saving project and try another name!");
                return;
            }

            var project = new Project { Name = dlg.Name, AppData = AppData.ToModel() };
            m_ProjectManager.Add(project);
            AppData.ResetHasUnsavedChanges();
            AddAndSaveLastProject(dlg.Name);
        }


        private void AddAndSaveLastProject(string pFileName)
        {
            m_ProjectHistory.Add(pFileName);
            _ = Task.Run(() => m_ProjectHistory.Save());
        }

        private void AddAndSaveLastScript(string pFileName)
        {
            m_ScriptHistory.Add(pFileName);
            _ = Task.Run(() => m_ScriptHistory.Save());
        }

        private void OnSaveProjectClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SaveProject.");
            // Is project already saved (by "Save as...")?
            if (m_ProjectManager.Current == null || m_ProjectManager.Current.Name == null)
            {
                // No! Project has not been saved yet.
                AddNewProject();
                return;
            }

            m_ProjectManager.Current.AppData = AppData.ToModel();
            m_ProjectManager.Update(m_ProjectManager.Current);
            AppData.ResetHasUnsavedChanges();
            AddAndSaveLastProject(m_ProjectManager.Current.Name);
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
            m_OpenFilesDialog.InitialDirectory = m_Config.ProjectsDirectory;
            if (m_OpenFilesDialog.ShowDialog() != true)
                return;

            LoadProject(m_OpenFilesDialog.FileName);
            DoesAllFilesAndDirectoriesExist();

            AddAndSaveLastProject(m_OpenFilesDialog.FileName);
        }


        /// <summary>
        /// Returns TRUE when all files and folders in project exist, else FALSE.
        /// </summary>
        /// <returns></returns>
        private bool DoesAllFilesAndDirectoriesExist()
        {
            var missingItems = FileSystemItemHelper.GetMissingItems(AppData);
            if (missingItems.Any())
            {
                string msg = m_MsgBoxMgr.GetMissingFilesMessage(missingItems);

                if (m_MsgBoxMgr.ShowMissingFilesOrDirsWarning(msg) == MessageBoxResult.Yes)
                {
                    RemoveDirsAndFiles(missingItems.ToArray());
                    return true;
                }
            }
            return false;
        }

        private void RemoveDirsAndFiles(IFileSystemItem[] pItems)
        {
            foreach (var f in pItems)
                AppData.Files.Remove(f as FileSystemItemVM);
        }

        private void LoadProject(string pName)
        {
            Log.Debug($"Loading project Name:'{pName}'");

            Project project = m_ProjectManager.Open(pName);
            if (project == null)
            {
                m_MsgBoxMgr.ShowError("Internal error!", "Project does not found!");
                return;
            }

            AppData.UpdateValues(project.AppData);
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
        {
            //=> OpenWebsiteInDefaultBrowser(@"https://github.com/pediRAM/GeNSIS/");
            var about = new AboutWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            _ = about.ShowDialog();
        }

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
                //_ = m_MsgBoxMgr.ShowNoGeneratedScriptFileError();
                OnSaveScriptAsClicked(sender, e);
                return;
            }

            File.WriteAllText(PathToGeneratedNsisScript, editor.Text, Encoding.UTF8);
            _ = m_MsgBoxMgr.ShowSavingScriptSucceededInfo();
        }

        private void OnSaveScriptAsClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked SaveScriptAs.");

            FileDialogHelper.InitDir(m_SaveScriptDialog, m_Config.ScriptsDirectory);
            m_SaveScriptDialog.FileName = PathHelper.GetNewScriptName(AppData);
            m_SaveScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;

            if (m_SaveScriptDialog.ShowDialog() != true)
                return;

            PathToGeneratedNsisScript = m_SaveScriptDialog.FileName;
            File.WriteAllText(PathToGeneratedNsisScript, editor.Text, Encoding.UTF8);
            AddAndSaveLastScript(m_SaveScriptDialog.FileName);
        }
        #endregion Methods

        private void OnOpenScriptClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked OpenScript.");
            m_OpenScriptDialog.Filter = FileDialogHelper.Filter.SCRIPT;
            FileDialogHelper.InitDir(m_OpenScriptDialog, PathHelper.GetScriptsDir());
            if (m_OpenScriptDialog.ShowDialog() != true)
                return;

            LoadScriptFromFile(m_OpenScriptDialog.FileName);
            AddAndSaveLastScript(m_OpenScriptDialog.FileName);
            PathToGeneratedNsisScript = m_OpenScriptDialog.FileName;
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

            if (ConfigHelper.GetLastImageFolder() == null)
                FileDialogHelper.InitDir(m_OpenImageDialog, ConfigHelper.GetNsisIconsFolder());
            else
                FileDialogHelper.InitDir(m_OpenImageDialog, ConfigHelper.GetLastImageFolder());

            if (m_OpenImageDialog.ShowDialog() == true)
            {
                ConfigHelper.SetLastImageFolder(Path.GetDirectoryName(m_OpenImageDialog.FileName));
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

            if (ConfigHelper.GetLastImageFolder() == null)
                m_OpenImageDialog.InitialDirectory = ConfigHelper.GetNsisWizardImagesFolder();
            else
                m_OpenImageDialog.InitialDirectory = ConfigHelper.GetLastImageFolder();

            if (m_OpenImageDialog.ShowDialog() == true)
            {
                ConfigHelper.SetLastImageFolder(Path.GetDirectoryName(m_OpenImageDialog.FileName));
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

            if (ConfigHelper.GetLastImageFolder() == null)
                m_OpenImageDialog.InitialDirectory = ConfigHelper.GetNsisHeaderImagesFolder();
            else
                m_OpenImageDialog.InitialDirectory = ConfigHelper.GetLastImageFolder();

            if (m_OpenImageDialog.ShowDialog() == true)
            {
                ConfigHelper.SetLastImageFolder(Path.GetDirectoryName(m_OpenImageDialog.FileName));
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
        private static readonly Regex m_RegexCommentLine = new Regex(@"^\s*;.*$", RegexOptions.Compiled);
        private void OnRemoveEmptyLinesAndCommentsClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked RemoveEmptyLinesAndComments.");
            var sb = new StringBuilder();
            foreach (string line in editor.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (m_RegexCommentLine.IsMatch(line)) continue;
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
                {
                    if (files.Count() == 1 && Directory.Exists(files.First()))
                    {
                        var result = m_MsgBoxMgr.ShowQuestion("Content or Directory?",
                            "Do you want to add the content in the directory\n" +
                            " or to add the directory (including the content)?\n\n" +

                            "Click 'Yes' to add the content, or\n" +
                            "click 'No' to add the whole directory, or\n" +
                            "click 'Cancel' to abort.", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            AddDirectoryContent(files.First());
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            AppData.AddFiles(files);
                        }
                        else return;
                    }

                }
            }
        }

        private async void OnScriptDropped(object sender, DragEventArgs e)
        {
            Log.Info("User dropped nsis script.");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (string path in paths)
                {
                    if (Path.GetExtension(paths[0]).Equals(".nsi", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await Dispatcher.BeginInvoke(() => LoadScriptFromFile(path));
                        AddAndSaveLastScript(path);
                        return;
                    }
                }
            }
        }

        private void OnPrintScriptClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked PrintScript.");

            try
            {
                //var printer = 
                // Create a PrintDialog
                PrintDialog printDialog = new PrintDialog();
                printDialog.CurrentPageEnabled = true;
                printDialog.SelectedPagesEnabled = true;
                printDialog.UserPageRangeEnabled = true;
                printDialog.PageRangeSelection = PageRangeSelection.AllPages;

                if (printDialog.ShowDialog() == true)
                {
                    // Create a FlowDocument with the content from the TextEditor
                    FlowDocument flowDocument = ICSharpCode.AvalonEdit.Utils.DocumentPrinter.CreateFlowDocumentForEditor(editor);
                    flowDocument.PageWidth = 8.27 * 96; // A4 width in pixels (96 pixels per inch)
                    flowDocument.PageHeight = 11.69 * 96; // A4 height in pixels (96 pixels per inch)
                    flowDocument.ColumnWidth = double.PositiveInfinity;
                    flowDocument.TextAlignment = TextAlignment.Left;
                    flowDocument.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                    flowDocument.FontSize = 12;
                    flowDocument.PagePadding = new Thickness(50);
                    // Create a PrintDocumentPaginator for printing
                    DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;

                    // Configure the printer to print in color
                    printDialog.PrintTicket.OutputColor = OutputColor.Color;

                    // Set up the printing process
                    printDialog.PrintDocument(paginator, "NSIS script");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private void OnFilesKeyDown(object sender, KeyEventArgs e)
        {
            Log.Info($"Files: User pressed Key:{e.Key}.");
            if (lsb_Files.SelectedItems.Count == 0)
                return;

            switch (e.Key)
            {
                case Key.A: // All (Select All).
                lsb_Files.SelectAll();
                break;

                case Key.C: // Clear (Remove All).
                lsb_Files.UnselectAll();
                break;

                case Key.N: // None (Select None / Unselect All).
                AppData.Files.Clear();
                break;

                case Key.S: // Sort.
                AppData.Files.Sort(AppData.Files.OrderBy(x => x.FSType).ThenBy(x => Path.GetExtension(x.Path)).ThenBy(x => x.Name));
                break;

                // Remove / Delete.
                case Key.R:
                case Key.Subtract:
                case Key.OemMinus:
                case Key.Delete:
                AppData.Files.RemoveRange(lsb_Files.GetSelectedItems<FileSystemItemVM>());
                break;
            }
        }

        private void OnFirewallRulesKeyDown(object sender, KeyEventArgs e)
        {
            Log.Info($"FirewallRules: User pressed Key:{e.Key}.");

            if (lsb_FirewallRules.SelectedItems.Count == 0)
                return;

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) && (e.Key == Key.A)))
                lsb_FirewallRules.SelectAll();

            if (e.Key == Key.Subtract || e.Key == Key.OemMinus || e.Key == Key.Delete)
                AppData.FirewallRules.RemoveRange(lsb_FirewallRules.GetSelectedItems<FirewallRuleVM>());
        }

        private void OnCheckProjectFilesClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked CheckProjectFiles.");
            DoesAllFilesAndDirectoriesExist();
        }

        private void OnExportAsPortableProjectClicked(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked ExportAsPortableProject.");

            // Do not continue if a single file/directory is missing!
            if (DoesAllFilesAndDirectoriesExist())
            {
                m_MsgBoxMgr.ShowInfo("Cannot export invalid project!", "Some files and/or folders are missing!\nCannot export project as portable project!");
                return;
            }

            // Where should the portable project file be saved?
            var dlg = CustomDialogHelper.CreateFilenameInputDialog("Project name", "Project name", "Please type a name for the exported project (only alphanumeric characers!):");
            if (dlg.ShowDialog() != true)
                return;

            // Name exists? => delete?
            if (m_ProjectManager.Contains(dlg.Text))
            {
                _ = m_MsgBoxMgr.ShowError("Project name already in use!", "Project name is already in use!\nPlease repeat the action and try another project name!");
                return;
            }

            AppData p = AppData.ToModel();
            p.Files.Clear();
            p.Sections.Clear();

            // todo: implement algo for choosing relative paths!
            var destDir = "dir";
            p.RelativePath = destDir;

            foreach (var f in AppData.Files)
            {
                string relPath = GetRelativePath(destDir, p, f);

                if (f.FSType == EFileSystemType.Directory)
                {
                    if (p.ExeName != null && p.ExeName.Path.Equals(f.Path, StringComparison.InvariantCultureIgnoreCase))
                    {
                        p.ExeName.Path = relPath;
                        p.ExeName.IsRelative = true;
                    }
                    else if (p.License != null && p.License.Path.Equals(f.Path, StringComparison.InvariantCultureIgnoreCase))
                    {
                        p.License.Path = relPath;
                        p.License.IsRelative = true;
                    }
                }
            }

            p.License.Path = GetRelativeImagePathOrNull(destDir, p.License.Path);
            p.License.IsRelative = true;

            p.InstallerIcon = GetRelativeImagePathOrNull(destDir, p.InstallerIcon);
            p.UninstallerIcon = GetRelativeImagePathOrNull(destDir, p.UninstallerIcon);

            p.InstallerHeaderImage = GetRelativeImagePathOrNull(destDir, p.InstallerHeaderImage);
            p.UninstallerHeaderImage = GetRelativeImagePathOrNull(destDir, p.UninstallerHeaderImage);

            p.InstallerWizardImage = GetRelativeImagePathOrNull(destDir, p.InstallerWizardImage);
            p.UninstallerWizardImage = GetRelativeImagePathOrNull(destDir, p.UninstallerWizardImage);

            var project = new Project { Name = dlg.Text, AppData = p };
            m_ProjectManager.Add(project);
            m_MsgBoxMgr.ShowInfo("Exporting project succeeded.", "Exporting the project as portable project succeeded.");
        }

        /// <summary>
        /// Creates and returns the relative path of copied image (to content dir, when exporting as portable project), or NULL.
        /// If image file exists at destination, it will not copy the file (overstepping copy).
        /// </summary>
        /// <param name="destDir">Path of content directory (export as portable project).</param>
        /// <param name="pImagePath">Path to image file (installer: icon, header, wizzard images).</param>
        /// <returns>Relative path or NULL.</returns>
        private string GetRelativeImagePathOrNull(string destDir, string pImagePath)
        {
            if (string.IsNullOrWhiteSpace(pImagePath) || !File.Exists(pImagePath))
                return null;

            FileInfo fi = new FileInfo(pImagePath);
            string newPath = $"{destDir}\\{fi.Name}";

            // Do not overwrite, because some images (like icon) could be used for installer and uninstaller!
            if (!File.Exists(newPath))
                File.Copy(fi.FullName, newPath);

            return Path.GetRelativePath(destDir, newPath);
        }

        private static string GetRelativePath(string destDir, AppData p, FileSystemItemVM f)
        {
            string newPath = $"{destDir}\\{f.Name}";

            if (f.FSType == EFileSystemType.File)
                File.Copy(f.Path, newPath);
            else if (f.FSType == EFileSystemType.Directory)
            {
                var di = new DirectoryInfo(f.Path);
                di.CopyTo(newPath, true);
            }
            var fsi = new FileSystemItem(newPath);
            var relPath = Path.GetRelativePath(destDir, newPath);
            fsi.Path = relPath;
            fsi.IsRelative = true;
            p.Files.Add(fsi);
            return relPath;
        }

        private void OnOpenScriptInExternEditor(object sender, RoutedEventArgs e)
        {
            Log.Info("User clicked OpenScriptInExternEditor.");
            OpenScriptInExternalEditor();
        }

        private void OpenFolderInExplorer(string pPath)
        {
            _ = Process.Start("explorer", $"\"{pPath}\"");
        }

        private void ShowFileInExplorer(string pPath)
        {
            _ = Process.Start("explorer", $"/select, \"{pPath}\"");
        }

        private void OnOpenInstallersFolderClicked(object sender, RoutedEventArgs e)
            => OpenFolderInExplorer(m_Config.InstallersDirectory);

        private void OnOpenProjectsFolderClicked(object sender, RoutedEventArgs e)
            => OpenFolderInExplorer(m_Config.ProjectsDirectory);

        private void OnOpenScriptsFolderClicked(object sender, RoutedEventArgs e)
            => OpenFolderInExplorer(m_Config.ScriptsDirectory);

        private void OnOpenMyComputerFolderClicked(object sender, RoutedEventArgs e)
            => OpenFolderInExplorer(Environment.GetFolderPath(Environment.SpecialFolder.MyComputer));

        private async void OnProjectHistoryClicked(object sender, RoutedEventArgs e)
        {
            var x = sender as System.Windows.Controls.MenuItem;
            if (x != null)
            {
                if (AppData.HasUnsavedChanges)
                {
                    if (m_MsgBoxMgr.ShowUnsavedChangesByOpenProjectWarning() != MessageBoxResult.Yes)
                        return;
                }

                await Dispatcher.BeginInvoke(() => LoadProject(x.Tag as string));
            }
        }

        private async void OnScriptHistoryClicked(object sender, RoutedEventArgs e)
        {
            var x = sender as System.Windows.Controls.MenuItem;
            if (x != null)
            {
                if (AppData.HasUnsavedChanges)
                {
                    if (m_MsgBoxMgr.ShowUnsavedChangesByOpenProjectWarning() != MessageBoxResult.Yes)
                        return;
                }

                await Dispatcher.BeginInvoke(() => LoadScriptFromFile(x.Tag as string));
            }
        }

        private void OnTestMarkdownClicked(object sender, RoutedEventArgs e)
        {
            mdxaml_ScrollViewer.Markdown = @"
# Title
Some text.

## Subtitle
An unordered list:
- one
- two
- thre

An ordered list:
1. one
1. two
1. three
1. four

*Links* **with** ***image*** [![faviicon](https://www.google.com/favicon.ico)](https://www.google.com ""google favicon"")

![attrnm](C:\Users\pedra\Pictures\2023-09-17 10_46_56-VirtualBoxVM.png)
";
        }
        private string m_LastSearchString;
        private async void OnTextEditorKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt)))
            {
                switch (e.Key)
                {
                    case Key.S: // Save / Save as...
                        {
                            e.Handled = true;
                            await Dispatcher.BeginInvoke(() => OnSaveScriptClicked(sender, e));
                        }
                        return;

                    case Key.F:
                        {
                            e.Handled = true;
                            if (editor.SelectionLength == 0)
                            {
                                tbxScriptSearchbox.SelectAll();
                                tbxScriptSearchbox.Focus();
                                return;
                            }
                            m_LastSearchString = editor.SelectedText;
                            tbxScriptSearchbox.Text = m_LastSearchString;
                            SearchText(editor.SelectedText);
                        }
                        return;

                    case Key.Add:
                    case Key.OemPlus:
                        {
                            e.Handled = true;
                            editor.FontSize = editor.FontSize + 2;
                            if (editor.FontSize > GConst.Editor.MAX_FONT_SIZE) editor.FontSize = GConst.Editor.MAX_FONT_SIZE;
                        }
                        return;

                    case Key.Subtract:
                    case Key.OemMinus:
                        {
                            e.Handled = true;
                            editor.FontSize = editor.FontSize - 2;
                            if (editor.FontSize < GConst.Editor.MIN_FONT_SIZE) editor.FontSize = GConst.Editor.MIN_FONT_SIZE;
                        }
                        return;

                    case Key.D0:
                        {
                            e.Handled = true;
                            editor.FontSize = GConst.Default.EDITOR_FONT_SIZE;
                        }
                        return;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.F3:
                        {
                            e.Handled = true;
                            if (string.IsNullOrEmpty(m_LastSearchString))
                            {
                                tbxScriptSearchbox.Focus();
                                return;
                            }
                            SearchText(m_LastSearchString);
                        }
                        return;
                }
            }
        }

        private void OnRemoveThisFile(object sender, RoutedEventArgs e)
        {
            FileSystemItemVM model = (sender as System.Windows.Controls.Button).Tag as FileSystemItemVM;
            if (m_MsgBoxMgr.ShowQuestion("User confirmation", $"Are you sure to remove \"{model.Name}\"?") == MessageBoxResult.Yes)
            {
                AppData.Files.Remove(model);
            }
        }

        private void OnRemoveThisFWRule(object sender, RoutedEventArgs e)
        {
            FirewallRuleVM model = (sender as System.Windows.Controls.Button).Tag as FirewallRuleVM;
            if (m_MsgBoxMgr.ShowQuestion("User confirmation", $"Are you sure to remove firewall-rule \"{model}\"?") == MessageBoxResult.Yes)
            {
                AppData.FirewallRules.Remove(model);
            }
        }

        private void OnRemoveThisComponent(object sender, RoutedEventArgs e)
        {
            SectionVM model = (sender as System.Windows.Controls.Button).Tag as SectionVM;
            if (m_MsgBoxMgr.ShowQuestion("User confirmation", $"Are you sure to remove component \"{model.Name}\"?") == MessageBoxResult.Yes)
            {
                AppData.Sections.Remove(model);
            }
        }

        private void OnAddThisLanguage(object sender, RoutedEventArgs e)
        {
            var selectedLanguage = (sender as System.Windows.Controls.Button).Tag as Language;
            LangSrc.Remove(selectedLanguage);
            LangDst.Add(selectedLanguage);
        }

        private void OnRemoveThisLanguage(object sender, RoutedEventArgs e)
        {
            var selectedLanguage = (sender as System.Windows.Controls.Button).Tag as Language;
            LangDst.Remove(selectedLanguage);
            LangSrc.Add(selectedLanguage);

        }

        private void OnSetThisFileAsExe(object sender, RoutedEventArgs e)
        {
            AppData.ExeName = (sender as System.Windows.Controls.Button).Tag as FileSystemItemVM;
        }

        private void OnSetThisFileAsLicense(object sender, RoutedEventArgs e)
        {
            AppData.License = (sender as System.Windows.Controls.Button).Tag as FileSystemItemVM;
        }

        private void OnOpenInBrowserDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            var fsItem = (sender as System.Windows.Controls.Button).Tag as FileSystemItemVM;
            if (fsItem == null) return;

            if (fsItem.FSType == EFileSystemType.Directory)
            {
                OpenFolderInExplorer((AppData as IAppData).GetFullPath(fsItem));
            }
            else
            {
                ShowFileInExplorer((AppData as IAppData).GetFullPath(fsItem));
            }
        }

        private int m_LastSearchIndex = 0;
        private void OnScriptSearchBoxClicked(object sender, KeyEventArgs e)
        {
            var tbx = (TextBox)sender;
            SearchText(tbx.Text);
        }

        private void SearchText(string pNeedle)
        {
            if (pNeedle == null || string.IsNullOrEmpty(pNeedle) || string.IsNullOrEmpty(editor.Text))
            {
                m_LastSearchIndex = 0;
                editor.SelectionLength = 0;
                return;
            }

            int index = editor.Text.IndexOf(pNeedle, m_LastSearchIndex, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                m_LastSearchIndex = 0;
                editor.SelectionLength = 0;
                return;
            }

            editor.SelectionStart = index;
            editor.SelectionLength = pNeedle.Length;
            ScrollToSelectedText(index);
            m_LastSearchIndex = index + 1;
        }



        private void ScrollToSelectedText(int offset)
        {
            TextArea textArea = editor.TextArea;
            if (textArea.Selection.IsEmpty)
                return;

            TextLocation startLocation = editor.Document.GetLocation(offset);
            textArea.Caret.BringCaretToView();
            editor.ScrollTo(startLocation.Line, startLocation.Column);
        }

        private void OnScriptSearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var tbx = (TextBox)sender;
            SearchText(tbx.Text);
        }

        private void OnDevTestClicked(object sender, RoutedEventArgs e)
        {
            var d = new CustomDialog();
            d.ShowDialog();
        }

        private void OnPreviewMouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                {
                    e.Handled = true;
                    editor.FontSize = editor.FontSize + 2;
                    if (editor.FontSize > GConst.Editor.MAX_FONT_SIZE)
                        editor.FontSize = GConst.Editor.MAX_FONT_SIZE;
                    return;
                }
                else if (e.Delta < 0)
                {
                    e.Handled = true;
                    editor.FontSize = editor.FontSize - 2;
                    if (editor.FontSize < GConst.Editor.MIN_FONT_SIZE)
                        editor.FontSize = GConst.Editor.MIN_FONT_SIZE;
                    return;
                }
            }
        }

        private void OnSavePageClicked(object sender, RoutedEventArgs e)
        {
            SettingGroup sg = pagePreviewUI.SettingGroup.ToModel();
            
        }

        private void OnClearPageClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnSaveDesignClicked(object sender, RoutedEventArgs e)
        {
            var design = new PageDesign
            {
                Installer = new PageImages
                {
                    Icon = AppData.InstallerIcon,
                    HeaderImage = AppData.InstallerHeaderImage,
                    WizardImage = AppData.InstallerWizardImage,
                },

                Uninstaller = new PageImages
                {
                    Icon = AppData.UninstallerIcon,
                    HeaderImage = AppData.UninstallerHeaderImage,
                    WizardImage = AppData.UninstallerWizardImage,
                }
            };

            design.Compensate();
            FilenameInputDialog dlg = CustomDialogHelper.CreateFilenameInputDialog("User input expected", "Save design", "Please type a name for your design (no special characters allowed!):");
            if (dlg.ShowDialog() != true)
                return;
            
            if (string.IsNullOrWhiteSpace(dlg.Text))
            {
                _ = m_MsgBoxMgr.ShowError("Illegal/Empty name!", "Name of design empty or whitespace!");
                return;
            }

            if (m_DesignManager.Contains(dlg.Text))
            {
                _ = m_MsgBoxMgr.ShowError("Design name already in use!", "Design name is already in use!\nPlease repeat the action and choose another name!");
                return;
            }
            design.Name = dlg.Text;
            m_DesignManager.Add(design);
        }
    }
}
