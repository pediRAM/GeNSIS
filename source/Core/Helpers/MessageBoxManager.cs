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


using System;
using System.Windows;

namespace GeNSIS.Core.Helpers
{
    internal class MessageBoxManager
    {
        internal MessageBoxResult ShowError(string pTitle, string pMessage, MessageBoxButton pButtons = MessageBoxButton.OK, MessageBoxImage pImage = MessageBoxImage.Error)
            => MessageBox.Show(pMessage, pTitle, pButtons, pImage);
        internal MessageBoxResult ShowInfo(string pTitle, string pMessage, MessageBoxButton pButtons = MessageBoxButton.OK, MessageBoxImage pImage = MessageBoxImage.Information)
            => MessageBox.Show(pMessage, pTitle, pButtons, pImage);
        internal MessageBoxResult ShowWarn(string pTitle, string pMessage, MessageBoxButton pButtons = MessageBoxButton.YesNo, MessageBoxImage pImage = MessageBoxImage.Warning)
            => MessageBox.Show(pMessage, pTitle, pButtons, pImage);

        internal MessageBoxResult ShowQuestion(string pTitle, string pMessage, MessageBoxButton pButtons = MessageBoxButton.YesNo, MessageBoxImage pImage = MessageBoxImage.Question)
            => MessageBox.Show(pMessage, pTitle, pButtons, pImage);

        internal MessageBoxResult ShowLoadConfigError(Exception pException)
            => ShowError("Loading configuration error!", $"While loading configuration file following exception was thrown:\n{pException.Message}");

        internal MessageBoxResult ShowInvalidDataError(string pError)
            => ShowError("Data validation failed!", pError);

        internal MessageBoxResult ShowNoGeneratedScriptFileError()
            => ShowError("No *.nsi file to compile!", "Path to currently generated NSIS script is empty!\nPlease generate the file in the previous tab first!");

        internal MessageBoxResult ShowGeneratedScriptFileNotFoundError()
            => ShowError("File not found!", "Generated NSIS script not found!");

        internal MessageBoxResult ShowUnsavedChangesByNewProjectWarning()
            => ShowWarn("Create new project", "Unsaved changes will be lost when you create new project!\nAre you sure you want to create a new project?");

        internal MessageBoxResult ShowUnsavedChangesByOpenProjectWarning()
            => ShowWarn("Open project", "Unsaved changes will be lost when you create new project!\nAre you sure you want to create a new project?");


        internal MessageBoxResult ShowUnsavedChangesByClosingAppWarning()
            => ShowWarn("Closing GeNSIS", "Unsaved changes will be lost if you close the application!\nAre you sure you want to close GeNSIS?");

        internal MessageBoxResult ShowSaveSettingChangesQuestion()
            => ShowQuestion("Save settings changes", "Some settings has been changed.\nDo you want to save settings?");

        internal MessageBoxResult ShowSettingsHasNoNsisPathDefError()
            => ShowError("Path of NSIS installation folder is empty or does not exist!\nPlease goto settings and set the NSIS installation folder first!", "Path not defined");

        internal MessageBoxResult ShowWizardImageBadSizeWarn()
            => ShowWarn("Image size does not match!", "The selected wizard image size is not 164 x 314 pixel!\nThis will cause errors during installer compilation by NSIS!\nPlease try to select the image after resizing it.", MessageBoxButton.OK);

        internal MessageBoxResult ShowBannerImageBadSizeWarn()
            => ShowWarn("Image size does not match!", "The selected wizard image size is not 150 x 57 pixel!\nThis will cause errors during installer compilation by NSIS!\nPlease try to select the image after resizing it.", MessageBoxButton.OK);

        internal MessageBoxResult ShowSavingScriptSucceededInfo()
            => ShowInfo("Saving file succeeded", "Script has been saved.");

        internal MessageBoxResult ShowDoYouWantToSelectNsisInstallDirManuallyQuestion()
            => ShowQuestion("Select directory manually", "NSIS installation directory not found!\nDo you want to browse and select it by yourself?", MessageBoxButton.YesNoCancel);

        internal MessageBoxResult ShowContinueWithoutNsisWarning()
            => ShowWarn("Missing NSIS compiler!", 
                "NSIS compiler not found!\n" +
                "You have two options:\n" +
                "1. Continue without NSIS compiler (YES)\n" +
                "2. Exit and install NSIS3 first then restart GeNSIS (NO)\n" +
                "\nDo you want to continue withouth compilation ability?");
    }
}
