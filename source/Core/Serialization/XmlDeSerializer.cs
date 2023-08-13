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


namespace GeNSIS.Core.Serialization
{
    using ExtendedXmlSerializer;
    using ExtendedXmlSerializer.Configuration;
    using GeNSIS.Core.Models;
    using GeNSIS.Core.ViewModels;
    using System.IO;
    using System.Xml;

    class XmlDeSerializer : IDeSerializer
    {
        public string DisplayName => "XML";

        public string Extension => ".xml";

        public Project ToProject(string pModelString)
            => GetExtendedSerializer().Deserialize<Project>(pModelString);

        public string ToString(Project project)
            => GetExtendedSerializer().Serialize(new XmlWriterSettings { Indent = true, IndentChars = "\t", NewLineOnAttributes = true}, project);

        private IExtendedXmlSerializer GetExtendedSerializer()
        {
            return new ConfigurationContainer()
                .EnableImplicitTyping(typeof(Project), typeof(AppData), typeof(FileSystemItem), typeof(FileSystemItemVM))
                .UseAutoFormatting()
                .UseOptimizedNamespaces()                
                .Create();
        }
    }
}
