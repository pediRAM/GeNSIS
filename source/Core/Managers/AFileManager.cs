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

namespace GeNSIS.Core.Managers
{
    using GeNSIS.Core.Serialization;
    using System.IO;


    public abstract class AFileManager<T>
    {
        protected string m_Path;

        /// <summary>
        /// Returns the path of file, where the object (by last call of <see cref="Load(string)"/> is/was saved.
        /// </summary>
        /// <returns></returns>
        public virtual string GetPath() => m_Path;

        /// <summary>
        /// Loads the the object of type T from given path.
        /// </summary>
        /// <param name="pPath">Filepath, where the object is saved.</param>
        /// <returns></returns>
        public virtual T Load(string pPath)
        {
            var provider = new DeSerializationProvider();
            var fileInfo = new FileInfo(pPath);
            var deserializer = provider.GetDeSerializerByExtension(fileInfo.Extension);
            m_Path = pPath;
            return deserializer.Deserialize<T>(File.ReadAllText(pPath, encoding: System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// Resets the filepath to NULL.
        /// </summary>
        public virtual void ResetPath() => m_Path = null;

        /// <summary>
        /// Saves the object at given path.
        /// </summary>
        /// <param name="pObject">An instance of type T to save.</param>
        /// <param name="pPath">Filepath, where the object should be saved.</param>
        public virtual void Save(T pObject, string pPath)
        {
            var provider = new DeSerializationProvider();
            var deserializer = provider.GetDeSerializerByExtension(Path.GetExtension(pPath));
            File.WriteAllText(pPath, deserializer.Serialize(pObject), encoding: System.Text.Encoding.UTF8);
        }
    }
}