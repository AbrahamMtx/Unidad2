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

namespace ServidorEncuesta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void iniciar_Click(object sender, RoutedEventArgs e)
        {
            iniciar.IsEnabled = false;
            detener.IsEnabled = true;
        }

        private void detener_Click(object sender, RoutedEventArgs e)
        {
            iniciar.IsEnabled = true;
            detener.IsEnabled = false;
        }
    }
}
