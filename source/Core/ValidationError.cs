﻿/*
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


namespace GeNSIS.Core
{
    public class ValidationError
    {

        #region Constructors
        public ValidationError(string pName, string pError, string pHint)
        {
            Name = pName;
            Error = pError;
            Hint = pHint;
        }
        #endregion Constructors


        #region Properties
        public string Name { get; }
        public string Error { get; }
        public string Hint { get; }
        #endregion Properties

        public override string ToString()
        {
            if(string.IsNullOrWhiteSpace(Hint))
                return $"Name:  {Name}\nError: {Error}";
            else
                return $"Name:  {Name}\nError: {Error}\nHint:  {Hint}";
        }
    }
}
