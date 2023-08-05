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


namespace GeNSIS.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    public abstract class AValueConverter : MarkupExtension, IValueConverter
    {
        public abstract object Convert(object pValue);

        public virtual object Convert(object pValue, Type pTargetType, object pParameter, CultureInfo pCulture)
            => Convert(pValue);

        public override object ProvideValue(IServiceProvider pServiceProvider)
            => this;

        public virtual object ConvertBack(object pValue, Type pTargetType, object pParameter, CultureInfo pCulture)
        {
            throw new InvalidOperationException($@"This AValueConverter inheritor: ({GetType().Name}) does not support converting types backward!");
        }
    }
}
