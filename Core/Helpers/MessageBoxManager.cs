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
        public MessageBoxResult ShowError(string pTitle, string pMessage, MessageBoxButton pButtons = MessageBoxButton.OK, MessageBoxImage pImage = MessageBoxImage.Error)
            => MessageBox.Show(pMessage, pTitle, pButtons, pImage);

        public MessageBoxResult ShowWarn(string pTitle, string pMessage, MessageBoxButton pButtons = MessageBoxButton.YesNo, MessageBoxImage pImage = MessageBoxImage.Warning)
            => MessageBox.Show(pMessage, pTitle, pButtons, pImage);

        public MessageBoxResult ShowQuestion(string pTitle, string pMessage, MessageBoxButton pButtons = MessageBoxButton.YesNo, MessageBoxImage pImage = MessageBoxImage.Question)
            => MessageBox.Show(pMessage, pTitle, pButtons, pImage);

        public MessageBoxResult ShowLoadConfigError(Exception pException)
            => ShowError("Loading configuration error!", $"While loading configuration file following exception was thrown:\n{pException.Message}");

        public MessageBoxResult ShowInvalidDataError(string pError)
            => ShowError("Data validation failed!", pError);

        public MessageBoxResult ShowNoGeneratedScriptFileError()
            => ShowError("No *.nsi file to compile!", "Path to currently generated NSIS script is empty!\nPlease generate the file in the previous tab first!");

        public MessageBoxResult ShowGeneratedScriptFileNotFoundError()
            => ShowError("File not found!", "Generated NSIS script not found!");

        public MessageBoxResult ShowUnsavedChangesByNewProjectWarning()
            => ShowWarn("Create new project", "Unsaved changes will be lost when you create new project!\nAre you sure you want to create a new project?");

        public MessageBoxResult ShowUnsavedChangesByOpenProjectWarning()
            => ShowWarn("Open project", "Unsaved changes will be lost when you create new project!\nAre you sure you want to create a new project?");


        public MessageBoxResult ShowUnsavedChangesByClosingAppWarning()
            => ShowWarn("Closing GeNSIS", "Unsaved changes will be lost if you close the application!\nAre you sure you want to create a new project?");

        public MessageBoxResult ShowSaveSettingChangesQuestion()
            => ShowQuestion("Save settings changes", "Some settings has been changed.\nDo you want to save settings?");

        public MessageBoxResult ShowSettingsHasNoNsisPathDefError()
            => ShowError("Path of NSIS installation folder is empty or does not exist!\nPlease goto settings and set the NSIS installation folder first!", "Path not defined");
    }
}
