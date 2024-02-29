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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CommonMath;
using WPF3D.Cameras;

namespace utCameras1
{
    public partial class MainWindow : Window
    {

        AmbientLight     ambient  = new AmbientLight ((Color)ColorConverter.ConvertFromString ("#404040"));
        DirectionalLight dir      = new DirectionalLight ((Color)ColorConverter.ConvertFromString ("#c0c0c0"), new Vector3D (2, -3, -1));
        Model3DGroup     lighting = new Model3DGroup ();
        ModelVisual3D    lightingVisual = new ModelVisual3D ();

        ProjectionCameraWrapper cameraWrapper;

        public MainWindow ()
        {
            InitializeComponent ();
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            {
                ModelVisual3D visual0 = new ModelVisual3D ();
                visual0.Content = ThreePlanes.MakeModel ();

                lighting.Children.Add (ambient);
                lighting.Children.Add (dir);
                lightingVisual.Content = lighting;

                cameraWrapper = new ProjectionCameraWrapper (ProjectionType.Perspective);
                view.Camera = cameraWrapper.Camera;

                view.Children.Add (visual0);
                view.Children.Add (lightingVisual);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        private void Button1_Click (object sender, RoutedEventArgs e)
        {
            Point3D pt = cameraWrapper.RelPosition;
            pt.X *= 2.5;
            pt.Y *= 2;
            pt.Z *= 1.5;
            cameraWrapper.RelPosition = pt;

            //Console.WriteLine ("camera rel pos: {0:0.0}", pt);

        }

        private void Button2_Click (object sender, RoutedEventArgs e)
        {
            Point3D pt = cameraWrapper.CenterOn;
            pt += new Vector3D (0.5, 0.7, 0.9);
            cameraWrapper.CenterOn = pt;
        }

        private void Button3_Click (object sender, RoutedEventArgs e)
        {
            if (cameraWrapper.FOV < 1)
                cameraWrapper.FOV = 45;
            else
                cameraWrapper.FOV /= 2;
        }

        private void Button4_Click (object sender, RoutedEventArgs e)
        {
            cameraWrapper.Theta -= 2;
            cameraWrapper.Phi -= 2;
        }

        private void Button5_Click (object sender, RoutedEventArgs e)
        {
            ////if (cameraWrapper.Rho > 5)
            ////    cameraWrapper.Rho -= 2;
            ////else
            ////    cameraWrapper.Rho = 30;
        }
    }
}
