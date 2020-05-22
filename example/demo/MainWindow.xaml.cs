using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(1111);
        }
    }
}