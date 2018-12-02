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
using UWPAppMgr.ViewModel;

namespace UWPAppMgr
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            ViewModel = this.DataContext as MainViewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Load();
        }
    }
}
