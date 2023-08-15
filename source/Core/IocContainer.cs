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


using System;
using System.Collections.Generic;
using System.Linq;

namespace GeNSIS.Core
{
    /// <summary>
    /// IOC Container Singleton: accecpts only class objects to put/get.
    /// </summary>
    public sealed class IocContainer : IDisposable
    {
        private Dictionary<Type, object> m_Dic = new Dictionary<Type, object>();

        #region Singleton

        private static bool m_HasDisposed = false;

        private static volatile IocContainer instance = null;
        private static readonly object syncObj = new object();

        private IocContainer()
        {
        }

        ~IocContainer()
        {
            Dispose();
        }


        public static IocContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                            instance = new IocContainer();
                    }
                }

                return instance;
            }
        }
        #endregion Singleton


        /// <summary>
        /// Returns the stored instance of type T if found, else NULL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            lock (syncObj)
            {
                if (m_Dic.ContainsKey(typeof(T)))
                    return (T)m_Dic[typeof(T)];

                return null;
            }
        }

        /// <summary>
        /// Contains total stored objects.
        /// </summary>
        public int Count => m_Dic.Count;

        /// <summary>
        /// Contains names of stored objects.
        /// </summary>
        public IEnumerable<string> Keys => m_Dic.Select((x, y) => x.Key.Name).ToArray();


        /// <summary>
        /// Returns TRUE if an instance of type T is already saved in store, else FALSE.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Contains<T>()
        {
            return Contains(typeof(T));
        }

        /// <summary>
        /// Returns TRUE if an instance of given type is already saved in store, else FALSE.
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public bool Contains(Type pType)
        {
            lock (syncObj)
            {
                return m_Dic.ContainsKey(pType);
            }
        }

        /// <summary>
        /// Adds/Overwrites given object to store.
        /// </summary>
        /// <param name="obj"></param>
        public void Put(object obj)
        {
            lock (syncObj)
            {
                m_Dic[obj.GetType()] = obj;
            }
        }

        /// <summary>
        /// Adds/Overwrites given object to store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Put<T>(object obj) where T : class
        {
            lock (syncObj)
            {
                m_Dic[typeof(T)] = obj;
            }
        }

        /// <summary>
        /// Returns TRUE if an instance of type T found and successfully removed from store, else FALSE.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Remove<T>() where T : class
        {
            lock (syncObj)
            {
                return m_Dic.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Disposes all disposable saved objects and clears the dictionary.
        /// </summary>
        public void Dispose()
        {
            if (m_HasDisposed)
                return;

            m_HasDisposed = true;

            foreach (var obj in m_Dic.Values)
            {
                (obj as IDisposable)?.Dispose();
            }
            m_Dic.Clear();
            m_Dic = null;
        }
    }
}
