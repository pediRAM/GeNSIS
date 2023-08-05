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


using GeNSIS.Core.ViewModels;

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
        {
            if (pValue == null) 
                return ValueWhenNullEmptyOrWhitespace;
            return string.IsNullOrWhiteSpace(((FileSystemItemVM)pValue).Path) ? ValueWhenNullEmptyOrWhitespace : ValueElse;
        }
    }
}
