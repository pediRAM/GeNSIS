namespace GeNSIS.UI.CustomDialogs
{
    internal class FilenameInputDialog : CustomDialog
    {
        private AlphaNumericBox m_TextBox;
        public FilenameInputDialog() : base()
        {
            m_TextBox = new AlphaNumericBox { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, VerticalAlignment = System.Windows.VerticalAlignment.Center, Margin = new System.Windows.Thickness(15)};
            FWElement = m_TextBox;
            DataContext = this;
        }

        public string Text => m_TextBox.Text;
    }
}
