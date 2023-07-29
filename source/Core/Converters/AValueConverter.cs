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
