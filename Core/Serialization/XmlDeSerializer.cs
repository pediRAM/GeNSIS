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
    using GeNSIS.Core.Models;
    using System.IO;
    using System.Xml.Serialization;

    class XmlDeSerializer : IDeSerializer
    {
        public string DisplayName => "XML";

        public string Extension => ".xml";

        public Project ToProject(string pModelString)
        {
            Project project = null;
            var xmlSerializer = new XmlSerializer(typeof(Project));

            using (var stringReader = new StringReader(pModelString))
            {
                project = (Project)xmlSerializer.Deserialize(stringReader);
            }
            return project;
        }

        public string ToString(Project project)
        {
            var xmlString = string.Empty;
            var xmlSerializer = new XmlSerializer(typeof(Project));

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, project);
                xmlString = stringWriter.ToString();
            }
            return xmlString;
        }
    }
}
