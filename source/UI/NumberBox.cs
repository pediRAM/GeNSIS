using GeNSIS.Core.Extensions;
using System.Windows.Input;

namespace GeNSIS.UI
{
    public class NumberBox : AlphaNumericBox
    {
        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !char.IsNumber(e.ToAlphaNumericChar());
        }
    }
}
