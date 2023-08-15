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
using GeNSIS.Core.Helpers;
using GeNSIS.Core.Models;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace GeNSIS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();
        private MessageBoxManager m_MsgBoxMgr;
        private Config m_Config;

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Debug("Starting up...");
            base.OnStartup(e);

            SetupExceptionHandling();
            CreateMessageBoxManager();
            LoadConfigurationFile();
            CheckNsisExists();
        }

        private void CheckNsisExists()
        {
            if (!NsisInstallationDirectoryExists())
            {
                if (m_MsgBoxMgr.ShowContinueWithoutNsisWarning() != MessageBoxResult.Yes)
                {
                    Log.Debug("User decided to quit due to missing NSIS installation.");
                    Shutdown();
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

                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                FileDialogHelper.InitDir(folderBrowserDialog, PathHelper.GetProgramFilesX86NsisDir());
                if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    Log?.Debug("User canceled searching for nsis install dir!");
                    return false;
                }

                if (Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.exe").Any(f => f.EndsWith(" \\nsis.exe", StringComparison.OrdinalIgnoreCase)))
                {
                    Log?.Debug("NSIS install dir chosen by user seems to be ok.");
                    m_Config.NsisInstallationDirectory = folderBrowserDialog.SelectedPath;

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

        private void LoadConfigurationFile()
        {
            Log.Debug("Loading application configuration file...");
            if (!ConfigHelper.ProcessAppConfig())
            {
                _ = m_MsgBoxMgr.ShowError("Loading/Creating configuration failed!", "Configuration file could neither be loaded nor created.\nApplication is going to close.");
                Log.Warn("Loading/Creating configuration file failed! Shutting down...");
                Shutdown();
            }
            else
            {
                m_Config = IocContainer.Instance.Get<Config>();
            }
        }

        private void CreateMessageBoxManager()
        {
            Log.Debug($"Creating {nameof(MessageBoxManager)}...");
            m_MsgBoxMgr = new MessageBoxManager();
            IocContainer.Instance.Put(m_MsgBoxMgr);
        }

        private void SetupExceptionHandling()
        {
            Log.Debug("Registering handlers for uncatched exceptions...");

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException(s, (Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(s, e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(s, e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(object sender, Exception exception, string source)
        {
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                var message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
                Log.Fatal($"Unhandled exception sender:{sender} (source:{source}), Message:{message}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
                m_MsgBoxMgr.ShowException(ex);
            }
            finally
            {
                Log.Fatal(exception);
                m_MsgBoxMgr.ShowException(exception);
            }
        }
    }
}