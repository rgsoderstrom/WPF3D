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
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WPF3D.Cameras;

namespace UnitTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            InitializeComponent ();
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            Console.WriteLine ("Unit Test here");

            try
            {
                ProjectionCameraWrapper camera = new ProjectionCameraWrapper ();

                //Console.WriteLine (string.Format ("Right {0:0.00}", camera.Right));
                //Console.WriteLine (string.Format ("Up {0:0.00}", camera.Up));

                Console.WriteLine (string.Format ("abs {0:0.00}", camera.AbsPosition));
                Console.WriteLine (string.Format ("rho {0:0.00}", camera.RelPositionRho));
                //Console.WriteLine (string.Format ("rho {0:0.00}", camera.RelPosition));
                Console.WriteLine ("----------------------");

                camera.RelPositionRho = 20;

                Console.WriteLine (string.Format ("abs {0:0.00}", camera.AbsPosition));
                Console.WriteLine (string.Format ("rho {0:0.00}", camera.RelPositionRho));
                //Console.WriteLine (string.Format ("rho {0:0.00}", camera.RelPosition));
                Console.WriteLine ("----------------------");
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }
    }
}
