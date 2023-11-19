using GeNSIS.Core.Extensions;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeNSIS.UI
{
    public class AlphaNumericTextBox : TextBox
    {
        public AlphaNumericTextBox() : base() 
        {
            KeyDown += OnKeyDown;
            DataContext = this;
        }

        protected virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
            char c = e.ToAlphaNumericChar();

            if (char.IsLetterOrDigit(c) || c == '_')
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
