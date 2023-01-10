using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace AxisRotationExperimenter
{
    internal class Wedge
    {
        // Wedge Vertices
        static Point3D [] wedgeVertices = {
            new Point3D ( 0.0,  0.0,  0.0), new Point3D ( 2.0,  0.0,  0.0), new Point3D ( 2.0,  1.0,  0.0), new Point3D ( 0.0,  0.0, -3.0),
            new Point3D ( 2.0,  0.0, -3.0), new Point3D ( 2.0,  1.0, -3.0), new Point3D ( 2.0,  1.0, -3.0), new Point3D ( 0.0,  0.0, -3.0),
            new Point3D ( 2.0,  1.0,  0.0), new Point3D ( 0.0,  0.0,  0.0), new Point3D ( 2.0,  1.0,  0.0), new Point3D ( 2.0,  0.0,  0.0),
            new Point3D ( 2.0,  1.0, -3.0), new Point3D ( 2.0,  0.0, -3.0), new Point3D ( 2.0,  0.0,  0.0), new Point3D ( 0.0,  0.0,  0.0),
            new Point3D ( 2.0,  0.0, -3.0), new Point3D ( 0.0,  0.0, -3.0),
        };

        // indices for triangles 
        static int [] triangleIndices = {3,5,4, 6,7,8, 7,9,8, 10,11,12, 11,13,12, 14,15,16, 15,17,16};
        static int [] topTriangleIndices = {0,1,2};

        static internal Model3DGroup MakeModel ()
        {
            MeshGeometry3D  bodyMesh  = new MeshGeometry3D ();
            GeometryModel3D bodyModel  = new GeometryModel3D ();
            MeshGeometry3D  topMesh  = new MeshGeometry3D ();
            GeometryModel3D topModel  = new GeometryModel3D ();

            Model3DGroup wedge = new Model3DGroup ();

            //***********************************************************

            // Body mesh
            bodyMesh.Positions = new Point3DCollection ();

            foreach (Point3D pt in wedgeVertices)
                bodyMesh.Positions.Add (pt);

            bodyMesh.TriangleIndices = new Int32Collection ();

            foreach (int i in triangleIndices)
                bodyMesh.TriangleIndices.Add (i);

            // Body Geometry Model
            bodyModel.Geometry = bodyMesh;
            bodyModel.Material = new DiffuseMaterial (Brushes.Cyan);
            bodyModel.BackMaterial = new DiffuseMaterial (Brushes.Red);

            wedge.Children.Add (bodyModel);

            //***********************************************************

            // Top mesh
            topMesh.Positions = new Point3DCollection ();

            foreach (Point3D pt in wedgeVertices)
                topMesh.Positions.Add (pt);

            topMesh.TriangleIndices = new Int32Collection ();

            foreach (int i in topTriangleIndices)
                topMesh.TriangleIndices.Add (i);

            // Top Geometry Model
            topModel.Geometry = topMesh;
            topModel.Material = new DiffuseMaterial (Brushes.Red);
            topModel.BackMaterial = new DiffuseMaterial (Brushes.Gray);

            wedge.Children.Add (topModel);


           // topModel.Transform = new TranslateTransform3D (-1, -0.5, 1.5);
           // bodyModel.Transform = topModel.Transform;

            return wedge;
        }
    }
}
