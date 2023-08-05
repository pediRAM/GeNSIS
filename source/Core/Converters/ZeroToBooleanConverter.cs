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
                case byte   : return ((byte)pValue   == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case sbyte  : return ((sbyte)pValue  == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case short  : return ((short)pValue  == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case ushort : return ((ushort)pValue == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case long   : return ((long)pValue   == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case ulong  : return ((ulong)pValue  == 0) ? ValueWhenZero: ValueWhenNotZero; 
                case int    : return ((int)pValue    == 0) ? ValueWhenZero: ValueWhenNotZero;
                case uint   : return ((uint)pValue   == 0) ? ValueWhenZero: ValueWhenNotZero;
                case double : return ((double)pValue == 0.0d) ? ValueWhenZero: ValueWhenNotZero;
            }

            throw new TypeAccessException("Unknown number type!");
        }
    }
}
