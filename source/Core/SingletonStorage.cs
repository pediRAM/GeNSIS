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

/*
using System;
using System.Collections.Generic;

namespace GeNSIS.Core
{
    /// <summary>
    /// Storage for reference type instances, which can save only 1 instance per given type.
    /// </summary>
    internal class SingletonStorage
    {
        private static readonly Dictionary<Type, object> s_Dic = new Dictionary<Type, object>();
        private static bool s_HasBeenDisposed;

        /// <summary>
        /// Saves the given instance of type T, and throws other instance out of storage (if exist).
        /// </summary>
        /// <typeparam name="T">Type T.</typeparam>
        /// <param name="obj">Instance of type T.</param>
        public void Put<T>(T obj) where T: class => s_Dic[typeof(T)] = obj;


        /// <summary>
        /// Saves the given instance of type T and returns TRUE if no older instance of same type is saved, else returns FALSE.
        /// </summary>
        /// <typeparam name="T">Type T.</typeparam>
        /// <param name="obj">Instance of type T.</param>
        /// <returns>TRUE if saved, FALSE if another instance of same type already exists.</returns>
        public bool PutIfNotContains<T>(T obj) where T: class
        {
            if (s_Dic.ContainsKey(typeof(T))) return false;
            s_Dic[typeof(T)] = obj;
            return true;
        }

        /// <summary>
        /// Returns instance of type T if exists, or throws Exception if no instance of type T found.
        /// </summary>
        /// <typeparam name="T">Type T.</typeparam>
        /// <returns>Instace of type T if found.</returns>
        public T Get<T>() where T : class => (T)s_Dic[typeof(T)];

        /// <summary>
        /// Returns the instance of type T if ound, else NULL.
        /// </summary>
        /// <typeparam name="T">Type of T.</typeparam>
        /// <returns>Instance of T or NULL when not found.</returns>
        public T GetObjectOrNull<T>() where T : class
        {
            if (!s_Dic.ContainsKey(typeof(T))) return null;
            return (T)s_Dic[typeof(T)];
        }

        /// <summary>
        /// Returns TRUE if instance of type T exists, else FALSE.
        /// </summary>
        /// <typeparam name="T">Type T.</typeparam>
        /// <param name="obj">Instance of type T or NULL if not found.</param>
        /// <returns>TRUE if found, FALSE else.</returns>
        public bool TryGet<T>(out T obj) where T : class
        {
            obj = null;
            if (!s_Dic.ContainsKey(typeof(T))) return false;
            obj = (T)s_Dic[typeof(T)];
            return true;
        }
    }
}
*/
