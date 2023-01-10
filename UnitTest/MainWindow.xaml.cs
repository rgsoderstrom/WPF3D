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
    public partial class MainWindow : Window
    {
        Point3D [] convexPoints = {
            new Point3D (0.707, 0.707, -4.0), new Point3D (0.707, 0.707, 0.0), new Point3D (0.383, 0.924, -4.0), new Point3D (0.383, 0.924, 0.0),
            new Point3D (0.383, 0.924, -4.0), new Point3D (0.383, 0.924, 0),   new Point3D (0, 1, -4),           new Point3D (0, 1, 0),
            new Point3D (0, 1.0, -4.0),       new Point3D (0, 1, 0),           new Point3D (-0.383, 0.924, -4),  new Point3D (-0.383, 0.924, 0), 
            new Point3D (-0.383, 0.924, -4),  new Point3D (-0.383, 0.924, 0),  new Point3D (-0.707, 0.707, -4),  new Point3D (-0.707, 0.707, 0),
        };

        int [] convexTriangles = {
             0,  2,  1,   1,  2,  3,
             4,  6,  5,   5,  6,  7,
             8, 10,  9,   9, 10, 11,
            12, 14, 13,  13, 14, 15
        };

        MeshGeometry3D  mesh   = new MeshGeometry3D ();
        GeometryModel3D model  = new GeometryModel3D ();
        ModelVisual3D   visual = new ModelVisual3D ();

        // transform for model rotation 
        AxisAngleRotation3D AAR = new AxisAngleRotation3D ();
        RotateTransform3D? rot = null;

        AmbientLight     ambient  = new AmbientLight ((Color)ColorConverter.ConvertFromString ("#404040"));
        DirectionalLight dir      = new DirectionalLight ((Color)ColorConverter.ConvertFromString ("#c0c0c0"), new Vector3D (2, -3, -1));
        Model3DGroup     lighting = new Model3DGroup ();
        ModelVisual3D    lightingVisual = new ModelVisual3D ();

        public MainWindow ()
        {
            InitializeComponent ();
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            {

                mesh.Positions = new Point3DCollection (convexPoints);
                mesh.TriangleIndices = new Int32Collection (convexTriangles);

                model.Geometry = mesh;
                model.Material = new DiffuseMaterial (Brushes.Cyan);
                model.BackMaterial = new DiffuseMaterial (Brushes.Pink);

                AAR.Axis = new Vector3D (1, 0, 0);
                rot = new RotateTransform3D (AAR);
                rot.CenterZ = -2;
                model.Transform = rot;

                visual.Content = model;

                lighting.Children.Add (ambient);
                lighting.Children.Add (dir);
                lightingVisual.Content = lighting;

                // camera
                view.Camera = new PerspectiveCamera (new Point3D (-2, 4, 4),
                                                     new Vector3D (0.4, -0.55, -1),
                                                     new Vector3D (0, 1, 0),
                                                     30);
                view.Children.Add (visual);
                view.Children.Add (lightingVisual);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }
    }
}
