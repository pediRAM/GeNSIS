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


using GeNSIS.Core.Interfaces;
using System.Windows;

namespace GeNSIS.Core.Converters
{
    public class NullToVisibilityConverter : AValueConverter
    {
        /// <summary>
        /// Gets/Sets the visibility when the given parameter is null. Default: Hidden.
        /// </summary>
        public Visibility VisibilityWhenNull { get; set; } = Visibility.Hidden;

        /// <summary>
        /// Gets/Sets the visibility when the given parameter is not null. Default: Visible.
        /// </summary>
        public Visibility VisibilityWhenNotNull { get; set; } = Visibility.Visible;

        public override object Convert(object pValue)
        {
            if (pValue == null)
                return VisibilityWhenNull;

            else if (pValue is IFileSystemItem)
                return string.IsNullOrWhiteSpace((pValue as IFileSystemItem).Path) ? VisibilityWhenNull : VisibilityWhenNotNull;
            else if (pValue is ISection)
                return string.IsNullOrWhiteSpace((pValue as ISection).SourcePath) ? VisibilityWhenNull : VisibilityWhenNotNull;
            else if (pValue is string)
                return string.IsNullOrWhiteSpace(pValue as string) ? VisibilityWhenNull : VisibilityWhenNotNull;
            return VisibilityWhenNotNull;
        }
    }
}
