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


using GeNSIS.Core.Models;
using GeNSIS.Core.Serialization;
using System.IO;

namespace GeNSIS.Core
{
    public class ProjectManager
    {
        private string m_ProjectFilePath;

        /// <summary>
        /// Returns the path of current project file if already saved, else NULL.
        /// </summary>
        /// <returns></returns>
        public string GetProjectFilePath() => m_ProjectFilePath;

        /// <summary>
        /// Resets the path of current project file to NULL.
        /// Call this whenever user creates a new project.
        /// </summary>
        public void ResetProjectFilePath() => m_ProjectFilePath = null;

        /// <summary>
        /// Loads the GeNSIS project from given path.
        /// </summary>
        /// <param name="pPath"></param>
        /// <returns></returns>
        public Project Load(string pPath)
        {
            var provider = new DeSerializationProvider();
            var fileInfo = new FileInfo(pPath);
            var deserializer = provider.GetDeSerializerByExtension(fileInfo.Extension);
            m_ProjectFilePath = pPath;
            return deserializer.ToProject(File.ReadAllText(pPath, encoding: System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// Saves given GeNSIS project to given path.
        /// </summary>
        /// <param name="pPath"></param>
        /// <param name="pProject"></param>
        public void Save(string pPath, Project pProject)
        {
            var provider = new DeSerializationProvider();
            var deserializer = provider.GetDeSerializerByExtension(Path.GetExtension(pPath));
            File.WriteAllText(pPath, deserializer.ToString(pProject), encoding: System.Text.Encoding.UTF8);
        }
    }
}
