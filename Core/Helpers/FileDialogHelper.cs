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


using System.IO;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace GeNSIS.Core.Helpers
{
    internal static class FileDialogHelper
    {
        public static class Filter
        {
            public const string BITMAP  = "Bitmap files|*.bmp";
            public const string ICON    = "Icon files|*.ico";
            public const string SCRIPT    = "NSIS files|*.nsi";
            public const string PROJECT = "XML files|*.xml";
        }

        public static void InitDir(SaveFileDialog pSfd, string pPath)
        {
            if (Directory.Exists(pPath))
                pSfd.InitialDirectory = pPath;
        }

        public static void InitDir(OpenFileDialog pOfd, string pPath)
        {
            if (Directory.Exists(pPath))
                pOfd.InitialDirectory = pPath;
        }

        public static void InitDir(FolderBrowserDialog pFbd, string pPath)
        {
            if (Directory.Exists(pPath))
                pFbd.InitialDirectory = pPath;
        }
    }
}
