using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;

namespace WPF3D.Lighting
{
    public class Lights
    {
        private   Model3DGroup model = new Model3DGroup ();        
        protected Model3DGroup Model {get {return model;}}

        private ModelVisual3D visual = new ModelVisual3D ();
        public  ModelVisual3D Visual {get {return visual;}}

        public Vector3D Direction
        {
            set
            {
                dir2.Direction = value;
            }
        }

        AmbientLight     ambient = new AmbientLight     ((Color)ColorConverter.ConvertFromString ("#505050"));
        DirectionalLight dir1    = new DirectionalLight ((Color)ColorConverter.ConvertFromString ("#a0a0a0"), new Vector3D (0.6, 0.6, -0.6));
        DirectionalLight dir2    = new DirectionalLight ((Color)ColorConverter.ConvertFromString ("#a0a0a0"), new Vector3D (1, 1, 1)); // direction set by client

        public Lights () : this (null)
        {
        }

        public Lights (Transform3DGroup xform)
        {
            Model.Children.Add (ambient);
            Model.Children.Add (dir1);
            Model.Children.Add (dir2);

            Model.Transform = xform;

            Visual.Content = Model;
        }
    }
}
