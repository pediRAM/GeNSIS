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

namespace GeNSIS.Core.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> pObservableCollection, IEnumerable<T> pItems)
        {
            foreach(var item in pItems)
                pObservableCollection.Add(item);
        }

    }

    public static class IEnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> pEnumerable)
            => pEnumerable.Count() == 0;

        public static bool HasElement<T>(this IEnumerable<T> pEnumerable)
            => pEnumerable != null && pEnumerable.Count() > 0;
    }
}
