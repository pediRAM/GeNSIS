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



namespace GeNSIS.Core.Converters
{
    internal class NullEmptyOrWhitespaceToBoolConverter : AValueConverter
    {
        /// <summary>
        /// Sets/Gets the value when the given string is null, empty or consists only of whitespace(s). Default: false.
        /// </summary>
        public bool ValueWhenNullEmptyOrWhitespace { get; set; } = false;

        /// <summary>
        /// Sets/Gets the value when the given string IS NOT null and IS NOT empty and DOES NOT CONSIST ONLY of whitespace(s). Default: true.
        /// </summary>
        public bool ValueElse { get; set; } = true;
        public override object Convert(object pValue)
            => (string.IsNullOrWhiteSpace((string)pValue)) ? ValueWhenNullEmptyOrWhitespace : ValueElse;
    }
}
