/***************************************************************************************
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
    using System.Runtime.CompilerServices;
    using System.Windows.Data;
    using System.Windows.Markup;
    public abstract class AValueConverter : MarkupExtension, IValueConverter
    {
        protected static readonly System.Windows.Shapes.Rectangle m_Dummy;

        static AValueConverter() 
            => m_Dummy ??= new System.Windows.Shapes.Rectangle();


        [MethodImpl(MethodImplOptions.NoInlining)]
        public abstract object Convert(object pValue);


        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual object Convert(object pValue, Type pTargetType, object pParameter, CultureInfo pCulture)
            => Convert(pValue);


        [MethodImpl(MethodImplOptions.NoInlining)]
        public override object ProvideValue(IServiceProvider pServiceProvider)
            => this;


        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual object ConvertBack(object pValue, Type pTargetType, object pParameter, CultureInfo pCulture)
        {
            throw new InvalidOperationException($@"This AValueConverter inheritor: ({GetType().Name}) does not support converting types backward!");
        }
    }
}
