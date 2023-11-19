using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeNSIS.UI
{
    /// <summary>
    /// Interaction logic for SettingsToolList.xaml
    /// </summary>
    public partial class SettingsToolList : UserControl
    {
        public SettingsToolList()
        {
            InitializeComponent();
        }

        private void OnBooleanMouseMoved(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(boolean, (sender as SettingPreviewUI).Clone(), DragDropEffects.Move);
        }


        private void OnStringMouseMoved(object sender, MouseEventArgs e)
        {

        }

        private void OnPasswordMouseMoved(object sender, MouseEventArgs e)
        {

        }

        private void OnFileMouseMoved(object sender, MouseEventArgs e)
        {

        }

        private void OnDirectoryMouseMoved(object sender, MouseEventArgs e)
        {

        }

        private void OnIntegerMouseMoved(object sender, MouseEventArgs e)
        {

        }

        private void OnNumberMouseMoved(object sender, MouseEventArgs e)
        {

        }
    }
}
