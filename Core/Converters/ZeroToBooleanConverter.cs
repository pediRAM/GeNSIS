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


using System;

namespace GeNSIS.Core.Converters
{
    public class ZeroToBooleanConverter : AValueConverter
    {
        /// <summary>
        /// Gets/Sets the value when the given parameter is zero. Default: false.
        /// </summary>
        public bool ValueWhenZero { get; set; } = false;

        /// <summary>
        /// Gets/Sets the value when the given parameter is not zero. Default: true.
        /// </summary>
        public bool ValueWhenNotZero { get; set; } = true;

        public override object Convert(object pValue)
        {
            switch(pValue)
            {
                case byte item: return ((byte)pValue == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case sbyte item: return ((sbyte)pValue == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case short item: return ((short)pValue == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case ushort item: return ((ushort)pValue == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case long item: return ((long)pValue == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case ulong item: return ((ulong)pValue == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case int item: return ((int)pValue == 0) ? ValueWhenZero: ValueWhenNotZero;
                case uint item: return ((uint)pValue == 0) ? ValueWhenZero: ValueWhenNotZero;
                case double item: return ((double)pValue == 0.0d) ? ValueWhenZero: ValueWhenNotZero;
            }

            throw new TypeAccessException("Unknown number type!");
        }
        
    }
}
