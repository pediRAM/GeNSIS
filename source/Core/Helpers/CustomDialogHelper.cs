using GeNSIS.UI.CustomDialogs;

namespace GeNSIS.Core.Helpers
{
    internal class CustomDialogHelper
    {
        public static FilenameInputDialog CreateFilenameInputDialog(string pWindowTitle, string pDialogTitle, string pDialogText) 
        {
            return new FilenameInputDialog
            {
                WindowTitle = pWindowTitle,
                DialogTitle = pDialogTitle,
                DialogText = pDialogText
            };
        }
    }
}
