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

namespace GeNSIS.Core.Serialization
{
    public class DeSerializationProvider
    {
        public IDeSerializer GetDeSerializerByExtension(string pExtension)
        {
            switch(pExtension.ToLower())
            {
                case ".json": return new JsonDeSerializer();
                case ".xml": return new XmlDeSerializer();
            }

            throw new ArgumentException($"Extension '{pExtension}' is unknown, illegal or no {nameof(IDeSerializer)} has been implemented for it yet!", nameof(pExtension));
        }

        public IDeSerializer GetDeSerializerByEnum(EDeSerializers pDeSerializerEnum)
        {
            switch (pDeSerializerEnum)
            {
                case EDeSerializers.JSON: return new JsonDeSerializer();
                case EDeSerializers.XML: return new XmlDeSerializer();
            }

            throw new NotImplementedException($"{nameof(IDeSerializer)} for {nameof(EDeSerializers)}.{pDeSerializerEnum} is not implemented!");
        }
    }
}
