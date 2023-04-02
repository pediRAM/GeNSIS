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


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace GeNSIS.Core.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach(var item in pItems)
                pObservableCollection.Add(item);
        }

        public static void RemoveRange<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems)
                pObservableCollection.Remove(item);
        }

        public static void MoveFirst<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems.Reverse())
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                pObservableCollection.Move(oldIndex, 0);
            }
        }

        public static void MovePrev<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems.Reverse())
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                if (oldIndex < 1) 
                    continue;
                pObservableCollection.Move(oldIndex, oldIndex - 1);
            }
        }

        public static void MoveLast<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems)
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                pObservableCollection.Move(oldIndex, pObservableCollection.Count - 1);
            }
        }

        public static void MoveNext<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems.Reverse())
            {
                int oldIndex = pObservableCollection.IndexOf(item);
                if (oldIndex >= pObservableCollection.Count - 1) 
                    continue;
                pObservableCollection.Move(oldIndex, oldIndex + 1);
            }
        }
    }

    public static class IEnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> pEnumerable)
            => pEnumerable.Count() == 0;

        public static bool HasElement<T>(this IEnumerable<T> pEnumerable)
            => pEnumerable != null && pEnumerable.Count() > 0;
    }

    public static class ListBoxExtensions
    {
        public static List<T> GetSelectedItems<T>(this ListBox pListBox)
        {
            var l = new List<T>();
            foreach (var item in pListBox.SelectedItems)
                l.Add((T)item);
            return l;
        }
    }
}
