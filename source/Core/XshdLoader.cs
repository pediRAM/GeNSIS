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


using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace GeNSIS.Core
{
    static class XshdLoader
    {
        private static XmlReader GetXmlReader(string pResourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            if (!pResourcePath.StartsWith(nameof(MainWindow)))
                pResourcePath = assembly.GetManifestResourceNames().Single(str => str.EndsWith(pResourcePath));

            var stream = assembly.GetManifestResourceStream(pResourcePath);
            return XmlReader.Create(stream);
        }

        public static IHighlightingDefinition LoadHighlightingDefinitionOrNull(string pFileName)
        {
            try
            {
                XmlReader xmlReader = GetXmlReader(pFileName);
                IHighlightingDefinition highlightingDefinition = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);
                xmlReader.Close();
                return highlightingDefinition;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }

            return null;            
        }
    }
}
