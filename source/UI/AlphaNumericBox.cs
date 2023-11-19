using GeNSIS.Core.Extensions;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeNSIS.UI
{
    public class AlphaNumericBox : TextBox
    {
        public AlphaNumericBox() : base() 
        {
            KeyDown += OnKeyDown;
            DataContext = this;
        }

        public virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            char c = e.ToAlphaNumericChar();

            if (char.IsLetterOrDigit(c) || c == '_')
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
