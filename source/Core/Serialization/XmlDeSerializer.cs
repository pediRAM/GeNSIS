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


namespace GeNSIS.Core.Serialization
{
    using ExtendedXmlSerializer;
    using ExtendedXmlSerializer.Configuration;
    using GeNSIS.Core.Models;
    using System.IO;
    using System.Xml;

    class XmlDeSerializer : IDeSerializer
    {
        public string DisplayName => "XML";

        public string Extension => ".xml";

        public Project ToProject(string pModelString)
            => GetExtendedSerializer().Deserialize<Project>(pModelString);

        public string ToString(Project project)
            => GetExtendedSerializer().Serialize(new XmlWriterSettings { Indent = true }, project);

        private IExtendedXmlSerializer GetExtendedSerializer()
        {
            return new ConfigurationContainer()
                .UseAutoFormatting()
                .UseOptimizedNamespaces()
                .EnableImplicitTyping(typeof(Project))
                .Create();
        }
    }
}
