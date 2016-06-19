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
using System.Windows.Shapes;

namespace pilipili
{
    /// <summary>
    /// InitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InitWindow : Window
    {
        public InitWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var roomId = int.Parse(textBox.Text);
                var mainWindow = new MainWindow(roomId);
                mainWindow.Show();
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("请输入合法的房间号");
            }
        }
    }
}
