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


namespace GeNSIS.Core.Serialization
{
    using System.Text.Json;

    public interface IDeSerializer
    {
        string DisplayName { get; }
        string Extension { get; }

        public T Deserialize<T>(string pSerializedModelAsString)
            => JsonSerializer.Deserialize<T>(pSerializedModelAsString) ?? default;

        public string Serialize<T>(T pModel)
        => JsonSerializer.Serialize(pModel, new JsonSerializerOptions { WriteIndented = true });

    }
}
