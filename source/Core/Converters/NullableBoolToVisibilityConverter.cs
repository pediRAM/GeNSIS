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


using System.Windows;

namespace GeNSIS.Core.Converters
{
    public class NullableBoolToVisibilityConverter : AValueConverter
    {
        /// <summary>
        /// The visibility when the parameter value is null. Default: Hidden.
        /// </summary>
        public Visibility VisibilityWhenNull { get; set; } = Visibility.Hidden;

        /// <summary>
        /// The visibility when the parameter value is false. Default: Hidden.
        /// </summary>
        public Visibility VisibilityWhenFalse { get; set; } = Visibility.Hidden;

        /// <summary>
        /// The visibility when the parameter value is true. Default: Visisble.
        /// </summary>
        public Visibility VisibilityWhenTrue { get; set; } = Visibility.Visible;

        public override object Convert(object pValue)
        {
            if (pValue == null) 
                return VisibilityWhenNull;

            return ((bool)pValue) ? VisibilityWhenTrue : VisibilityWhenFalse;
        }
    }
}
