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
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace GeNSIS
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            Title = $"GeNSIS {AsmConst.FULL_VERSION}";
            MouseDown += OnMouseDown;
            DataContext = this;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }

        public string Product { get; set; } = $"GeNSIS (GeNSIS.exe)";
        public string Version { get; set; } = AsmConst.FULL_VERSION;
        public string Copyright { get; set; } = DateTime.Now.Year == 2023 ? $"Copyright ©{DateTime.Now.Year} Pedram GANJEH HADIDI" : $"Copyright ©2023-{DateTime.Now.Year} Pedram GANJEH HADIDI";

        public string GPL { get; set; } = @"GeNSIS (GEnerates NullSoft Installer Script)
Copyright © 2023 Pedram GANJEH HADIDI

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


******************************************************************************
***                Third Party Libraries, Tools and Content                ***
******************************************************************************
GeNSIS and the documentations could not be created without following icons, tools, and libraries:

 * NSIS: zlib/libpng license, copyright by contributors
 * Gnu Image Manipulation Program (GIMP): GPL-3.0 license, copyright by Spencer KIMBALL & Peter MATTIS
 * Nuvola icons: LGPL 2.1 license and copyright by David VIGNONI the artist and creator of Nuvola icons
 * Greenshot (screenshot tool): GPL-3.0 license and copyright by Robin KROM
 * AvalonEdit (lib): MIT license and copyright by Daniel GRUNWALD (source on Github)
 * Visual Studio 2022 Community (IDE): licensed under MICROSOFT VISUAL STUDIO COMMUNITY 2022 copyright by Microsoft
 * Visual Studio Code (IDE): licensed under MICROSOFT VISUAL STUDIO CODE
 * ExtendedXmlSerializer (lib): MIT license and copyright by Wojciech NAGÓRSKI & Michael DEMOND
 * NLog (logger lib): BSD-3-Cluase license, copyright by Jarek KOWALSKI, Kim CHRISTENSEN, Julian VERDURMEN
";

        private void OnOpenHomepageClicked(object sender, RoutedEventArgs e)
            => OpenWebsiteInDefaultBrowser(@"https://github.com/pediRAM/GeNSIS/");

        private void OpenWebsiteInDefaultBrowser(string pUrl)
            => _ = Process.Start(new ProcessStartInfo(pUrl) { UseShellExecute = true });
    }
}
