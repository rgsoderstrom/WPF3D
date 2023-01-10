using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace utCameras1
{
    internal class ThreePlanes
    { 
        static Point3D [] PositionsX0 = {new Point3D ( 0, -1,  1), new Point3D ( 0, 1, 1), new Point3D ( 0, 1, -1), new Point3D (0, -1, -1)};
        static Point3D [] PositionsY0 = {new Point3D ( 1,  0,  1), new Point3D (-1, 0, 1), new Point3D (-1, 0, -1), new Point3D (1,  0, -1)};
        static Point3D [] PositionsZ0 = {new Point3D (-1, -1,  0), new Point3D (-1, 1, 0), new Point3D ( 1, 1,  0), new Point3D (1, -1,  0)};

        static int [] TriangleIndices = {0, 2, 1,  0, 3, 2};

        static MeshGeometry3D  mesh0   = new MeshGeometry3D ();
        static MeshGeometry3D  mesh1   = new MeshGeometry3D ();
        static MeshGeometry3D  mesh2   = new MeshGeometry3D ();

        static GeometryModel3D model0  = new GeometryModel3D ();
        static GeometryModel3D model1  = new GeometryModel3D ();
        static GeometryModel3D model2  = new GeometryModel3D ();

        static internal Model3DGroup MakeModel ()
        {
            mesh0.Positions = new Point3DCollection (PositionsX0);
            mesh1.Positions = new Point3DCollection (PositionsY0);
            mesh2.Positions = new Point3DCollection (PositionsZ0);

            mesh0.TriangleIndices = new Int32Collection (TriangleIndices);
            mesh1.TriangleIndices = new Int32Collection (TriangleIndices);
            mesh2.TriangleIndices = new Int32Collection (TriangleIndices);

            model0.Geometry = mesh0;
            model0.Material = new DiffuseMaterial (Brushes.Red);
            model0.BackMaterial = new DiffuseMaterial (Brushes.Salmon);

            model1.Geometry = mesh1;
            model1.Material = new DiffuseMaterial (Brushes.Green);
            model1.BackMaterial = new DiffuseMaterial (Brushes.LightGreen);

            model2.Geometry = mesh2;
            model2.Material = new DiffuseMaterial (Brushes.Blue);
            model2.BackMaterial = new DiffuseMaterial (Brushes.LightBlue);

            Model3DGroup AllThree = new Model3DGroup ();
            AllThree.Children.Add (model0);
            AllThree.Children.Add (model1);
            AllThree.Children.Add (model2);

            return AllThree;
        }
    }
}
