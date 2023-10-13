namespace GeNSIS.Core.TextGenerators
{
    using GeNSIS.Core.Interfaces;
    using System.Text;

    /*
     * Contains methods to add lines, sections, comments and indent lines.
     */
    public partial class NsisGenerator : ITextGenerator
    {
        #region Code Line Adding Methods
        private void Add()
        {
            sb.AppendLine();
            ln++;
        }

        private void Add(string s)
        {
            sb.AppendLine($"{Indent}{s}");
            ln++;
        }

        private bool HasSectionStarted { get; set; }
        private string Indent => HasSectionStarted ? "    " : string.Empty;
        private void AddSection(string pParameters)
        {
            Add($"Section {pParameters}");
            HasSectionStarted = true;
        }

        private void AddFunction(string pParameters)
        {
            Add($"Function {pParameters}");
            HasSectionStarted = true;
        }

        private void AddSection()
        {
            Add("Section");
            HasSectionStarted = true;
        }

        private void AddFunction()
        {
            Add("Function");
            HasSectionStarted = true;
        }

        private void AddSectionEnd()
        {
            HasSectionStarted = false;
            Add($"SectionEnd");
        }

        private void AddFunctionEnd()
        {
            HasSectionStarted = false;
            Add($"FunctionEnd");
        }

        private void AddComment(string pCommentLine)
        {
            sb.AppendLine($"{Indent}; {pCommentLine}");
            ln++;
        }

        private void AddLabel(string pName) => Add($"{pName}:");

        private void AddCommentBlock(params string[] pCommentLines)
        {
            foreach (var s in pCommentLines)
                AddComment(s);
        }

        private void AddEmptyComment() => AddComment(string.Empty);

        private void AddStripline(int pPadRight = STRIPLINE_LENGTH)
        {
            Add($";".PadRight(pPadRight, '-'));
        }

        private void AddDefine(string pVarName, string pValue)
        {
            sb.AppendLine($"!define {pVarName} \"{pValue}\"");
            ln++;
        }

        private void AddDefine(string pVarName)
        {
            sb.AppendLine($"!define {pVarName}");
            ln++;
        }

        private void AddLog(string pValue)
        {
            sb.AppendLine($"!echo \"{pValue}\"");
            ln++;
        }

        private void AddInsertMacro(string pMacro, string pValue)
        {
            sb.AppendLine($"!insertmacro {pMacro} \"{pValue}\"");
            ln++;
        }

        private void AddInsertMacro(string pMacro)
        {
            sb.AppendLine($"!insertmacro {pMacro}");
            ln++;
        }
        #endregion Code Line Adding Methods
    }
}
