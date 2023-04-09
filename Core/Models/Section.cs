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
    #region Usings
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.ViewModels;
    using System.Xml;
    using System.Xml.Serialization;
    #endregion Usings

    [XmlRoot]
    public class Section : ISection
    {
        #region Ctors
        public Section() { }

        public Section(ISection pSection) : this()
        {
            UpdateValues(pSection);
        }
        #endregion Ctors



        #region Properties
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string SourcePath { get; set; }

        [XmlElement]
        public string TargetInstallDir { get; set; }

        #endregion Properties

        #region Functions
        public SectionVM ToViewModel()
        {
            // todo: check and review Clone() function!
            return new SectionVM
            {
                Name = Name,
                SourcePath = SourcePath,
                TargetInstallDir = TargetInstallDir,
            };
        }

        public void UpdateValues(ISection s)
        {
            Name = s.Name;
            SourcePath = s.SourcePath;
            TargetInstallDir = s.TargetInstallDir;
        }

        #endregion Functions
    }
}
