using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
