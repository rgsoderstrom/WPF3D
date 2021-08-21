
using System;
using System.Windows;  // for DependencyObject
using System.Windows.Media.Media3D; // need PresentationFramework reference
using System.Windows.Input; // key event args

using WPF3D.Lighting;

namespace WPF3D.Cameras
{
    public class ProjectionCameraWrapper : DependencyObject
    {
        static double DefaultPhi = 60; // degrees
        static double DefaultTheta = 30;

        //***************************************************************************************
        //
        // Camera position and orientation
        //
        public ProjectionCamera Camera {get; private set;}

        // public properties for these defined below
        double rho = 12;
        double fov = 45;   // field of view, degrees, for perspective camera
        double width = 7;  // for orthographic camera

        protected Point3D eyePosition
        {
            get
            {
                return new Point3D (0, 0, Rho);
            }
        }

        // eye coordinate orientation when (theta == 0) and (phi == 0)
        readonly Vector3D eyeRight = new Vector3D  (0, 1, 0); // eye X axis
        readonly Vector3D eyeUp    = new Vector3D (-1, 0, 0); // eye Y axis
        readonly Vector3D eyeLook  = new Vector3D  (0, 0,-1); // eye Z axis

        AxisAngleRotation3D  cameraPhiAAR   = new AxisAngleRotation3D ();
        AxisAngleRotation3D  cameraThetaAAR = new AxisAngleRotation3D ();
        TranslateTransform3D cameraMove     = new TranslateTransform3D ();

        protected Transform3DGroup cameraGroup = new Transform3DGroup ();

        public Transform3DGroup CameraTransform
        {
            get {return cameraGroup;}
        }

        //***************************************************************************************
        //
        // Lighting that moves with camera (like a headlamp)
        //
        Lights lighting = null; // optional, may remain null

        //***************************************************************************************
        //
        // Public access to Camera position and orientation
        //
        public Point3D  Position  {get {return cameraGroup.Transform (eyePosition);}}
        public Vector3D Direction {get {return cameraGroup.Transform (-1 * (Vector3D) eyePosition);}}
        public Vector3D Up        {get {return cameraGroup.Transform (eyeUp);}}
        public Vector3D Right     {get {return cameraGroup.Transform (eyeRight);}}

        public static readonly DependencyProperty PhiProperty = DependencyProperty.Register("Phi", typeof(double), typeof(ProjectionCameraWrapper),                
                                                                                            new PropertyMetadata(DefaultPhi, PropertyChanged));
        public double Phi
        {
            set {SetValue(PhiProperty, value);}
            get {return (double)GetValue(PhiProperty);}
        }

        public static readonly DependencyProperty ThetaProperty = DependencyProperty.Register("Theta", typeof(double), typeof(ProjectionCameraWrapper),                
                                                                                            new PropertyMetadata(DefaultTheta, PropertyChanged));
        public double Theta
        {
            set {SetValue(ThetaProperty, value);}
            get {return (double)GetValue(ThetaProperty);}
        }

        //**************************************************************************************

        private static void PropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ProjectionCameraWrapper).PropertyChanged (e);
        }

        private void PropertyChanged (DependencyPropertyChangedEventArgs args)
        {
            if (args.Property == PhiProperty)   ProcessNewPhi ();
            if (args.Property == ThetaProperty) ProcessNewTheta ();
        }

        //**************************************************************************************

        public ProjectionCameraWrapper () : this (ProjectionType.Perspective)
        {
        }

        public ProjectionCameraWrapper (ProjectionType projection, Lights l = null)
        {   
            //********************************************************************************
            //
            // Initialize camera position and orientation
            // 
            cameraPhiAAR.Axis = new Vector3D (0,1,0);
            cameraPhiAAR.Angle = Phi;
            RotateTransform3D    rotation1 = new RotateTransform3D ();
            rotation1.Rotation = cameraPhiAAR;

            cameraThetaAAR.Axis = new Vector3D (0,0,1);
            cameraThetaAAR.Angle = Theta;
            RotateTransform3D    rotation2 = new RotateTransform3D ();
            rotation2.Rotation = cameraThetaAAR;

            cameraGroup.Children.Add (rotation1);
            cameraGroup.Children.Add (rotation2);
            cameraGroup.Children.Add (cameraMove);

            Vector3D _eyeLook    = cameraGroup.Transform (eyeLook);
            Vector3D _eyeUp      = cameraGroup.Transform (eyeUp);
            Vector3D _eyeRight   = cameraGroup.Transform (eyeRight);
            Point3D _eyePosition = cameraGroup.Transform (eyePosition);

            if (projection == ProjectionType.Perspective)
                Camera = new PerspectiveCamera (_eyePosition, _eyeLook, _eyeUp, fov);
            else
                Camera = new OrthographicCamera (_eyePosition, _eyeLook, _eyeUp, width);

            Camera.NearPlaneDistance = 1;
            Camera.FarPlaneDistance = 100;

            lighting = l; 
        }

        //*********************************************************************************************

        void ProcessNewPhi ()
        {
            cameraPhiAAR.Angle = Phi;

            Vector3D _eyeLook     = cameraGroup.Transform (eyeLook);
            Vector3D _eyeUp       = cameraGroup.Transform (eyeUp);
            Point3D  _eyePosition = cameraGroup.Transform (eyePosition);

            Camera.Position      = _eyePosition;
            Camera.LookDirection = _eyeLook;
            Camera.UpDirection   = _eyeUp;

            if (lighting != null)
                lighting.Direction = _eyeLook;
        }

        //*********************************************************************************************

        void ProcessNewTheta ()
        {
            cameraThetaAAR.Angle = Theta;

            Vector3D _eyeLook     = cameraGroup.Transform (eyeLook);
            Vector3D _eyeUp       = cameraGroup.Transform (eyeUp);
            Point3D  _eyePosition = cameraGroup.Transform (eyePosition);

            Camera.Position      = _eyePosition;
            Camera.LookDirection = _eyeLook;
            Camera.UpDirection   = _eyeUp; 

            if (lighting != null)
                lighting.Direction = _eyeLook;
        }

        //*********************************************************************************************

        public double Rho
        {
            get
            {
                return rho;
            }

            set
            {
                rho = value;

                Point3D _eyePosition = cameraGroup.Transform (eyePosition);
                Camera.Position = _eyePosition;
            }
        }

        //*********************************************************************************************

        public double FOV
        {
            get
            {
                return fov;
            }

            set
            {
                fov = value;
                
                if (Camera is PerspectiveCamera)
                    (Camera as PerspectiveCamera).FieldOfView = fov;
            }
        }

        //*********************************************************************************************

        public double Width
        {
            get
            {
                return width;
            }

            set
            {
                width = value;
                
                if (Camera is OrthographicCamera)
                    (Camera as OrthographicCamera).Width = width;
            }
        }

        //*********************************************************************************************

        public double X {get {return rho * Math.Sin (Phi * Math.PI / 180) * Math.Cos (Theta * Math.PI / 180);}}
        public double Y {get {return rho * Math.Sin (Phi * Math.PI / 180) * Math.Sin (Theta * Math.PI / 180);}}
        public double Z {get {return rho * Math.Cos (Phi * Math.PI / 180);}}

        //*********************************************************************************************

        public void LookAt (Point3D pt)
        {
            LookAt (pt.X, pt.Y, pt.Z);
        }

        public void LookAt (double x, double y, double z)
        {
            cameraMove.OffsetX = x;
            cameraMove.OffsetY = y;
            cameraMove.OffsetZ = z;

            Point3D _eyePosition = cameraGroup.Transform (eyePosition);
            Camera.Position = _eyePosition;
        }

        //
        // MoveByFraction () -
        //
        public void MoveByFraction (double xFraction, double yFraction, double zFraction)
        {
            double angle = (FOV / 2) * (Math.PI / 180);
            double One = (Camera is PerspectiveCamera) ? (Math.Tan (angle) * Rho) : (Width / 2);

            MoveBy (xFraction * One, yFraction * One, zFraction * One);
        }

        public void MoveBy (double dx, double dy, double dz)
        {
            Vector3D _eyeRight = cameraGroup.Transform (eyeRight);
            Vector3D _eyeUp    = cameraGroup.Transform (eyeUp);
            Vector3D _eyeLook  = cameraGroup.Transform (eyeLook);

            Point3D lookingAt = new Point3D (cameraMove.OffsetX, cameraMove.OffsetY, cameraMove.OffsetZ)
                              + dx * _eyeRight
                              + dy * _eyeUp
                              + dz * _eyeLook;

            cameraMove.OffsetX = lookingAt.X;
            cameraMove.OffsetY = lookingAt.Y;
            cameraMove.OffsetZ = lookingAt.Z;

            Point3D _eyePosition = cameraGroup.Transform (eyePosition);
            Camera.Position = _eyePosition;
        }

        //*********************************************************************************************

        public void OnKeyDown (KeyEventArgs args)
        {
            const double step = 0.01;

            switch (args.Key)
            {
                case Key.Left:
                case Key.NumPad4:
                    MoveByFraction (step, 0, 0);
                    break;

                case Key.Right:
                case Key.NumPad6:
                    MoveByFraction (-step, 0, 0);
                    break;

                case Key.Up:
                case Key.NumPad8:
                    MoveByFraction (0,-step,0);
                    break;

                case Key.Down:
                case Key.NumPad2:
                    MoveByFraction (0,step,0);
                    break;

                case Key.Subtract:
                    MoveByFraction (0,0,-step);
                    break;

                case Key.Add:
                    MoveByFraction (0,0,step);
                    break;
            }
        }

        //*********************************************************************************************
    }
}
