
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
