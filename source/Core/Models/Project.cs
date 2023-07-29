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


namespace GeNSIS.Core.Models
{
    using System.Xml.Serialization;

    [XmlRoot]
    public class Project
    {
        [XmlElement]
        public string Version { get; set; } = AsmConst.MODEL_VERSION;

        //[XmlElement]
        //public string Note { get; set; }

        [XmlElement]
        public AppData AppData { get; set; } = new AppData();

        public ProjectVM ToViewModel()
        {
            return new ProjectVM
            {
                Version = Version,
                //Note = Note,
                AppData = AppData.ToViewModel()
            };
        }

    }
}
