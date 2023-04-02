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
using System.Collections.Generic;
using System.Linq;

namespace GeNSIS.Core
{
    public class IocFactory
    {
        private static readonly Dictionary<Type, Func<object>> s_Factories = new Dictionary<Type, Func<object>>();
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            s_Factories[typeof(TInterface)] = () => Resolve(typeof(TImplementation));
        }
        public TInterface Resolve<TInterface>()
        {
            return (TInterface)Resolve(typeof(TInterface));
        }
        private object Resolve(Type type)
        {
            Func<object> factory;
            if (s_Factories.TryGetValue(type, out factory))
            {
                return factory();
            }
            if (type.IsInterface || type.IsAbstract)
            {
                throw new Exception($"No registration for type '{type.FullName}' found.");
            }
            var constructor = type.GetConstructors().Single();
            var parameters = constructor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
            return Activator.CreateInstance(type, parameters);
        }
    }

}
