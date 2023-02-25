﻿/***************************************************************************************
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



namespace GeNSIS.Core
{
    using GeNSIS.Core.Models;

    public class TextGenerator
    {
        public virtual string Generate(AppData pAppData)
        {
            throw new System.NotImplementedException();
        }
        public virtual string GetCommentLine(string pCommentLine) => $"; {pCommentLine}";
        public virtual string GetCommentHorizontalRuler(int pLength = 80) => ";".PadLeft(pLength, '*');
        public virtual string GetDefine(string pVarName, string pValue) => $"!define {pVarName} \"{pValue}\"";
        public virtual string GetEcho(string pValue) => $"!echo \"{pValue}\"";
        public virtual string GetInsertMacro(string pMacro, string pValue) 
            => $"!insertmacro {pMacro} \"{pValue}\"";
        public virtual string GetInsertMacro(string pMacro)
            => $"!insertmacro {pMacro}";

    }
}
