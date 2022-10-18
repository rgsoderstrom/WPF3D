
using System;
using System.Windows;  // for DependencyObject
using System.Windows.Media.Media3D; // need PresentationFramework reference
using System.Windows.Input; // key event args

using Common;
using CommonMath;
using WPF3D.Lighting;

namespace WPF3D.Cameras
{
    public class ProjectionCameraWrapper : DependencyObject
    {
        static double DefaultPhi   = 60; // degrees
        static double DefaultTheta = 40;
        static double DefaultRho   = 10;

        // this will be set to PerspectiveCamera or OrthographicCamera in ctor
        public ProjectionCamera Camera {get; protected set;}

        // Camera position relative to ViewCenter
        Point3D RelPosition = new Point3D (DefaultRho * Math.Sin (DefaultPhi * Math.PI / 180) * Math.Cos (DefaultTheta * Math.PI / 180),
                                           DefaultRho * Math.Sin (DefaultPhi * Math.PI / 180) * Math.Sin (DefaultTheta * Math.PI / 180),
                                           DefaultRho * Math.Cos (DefaultPhi * Math.PI / 180));



        public double RelPositionRho
        {
            get {return ((Vector3D) RelPosition).Length;}
            
            set 
            {
                double multiplier = value / RelPositionRho;
                RelPosition.X *= multiplier;
                RelPosition.Y *= multiplier;
                RelPosition.Z *= multiplier;
                UpdateCamera ();
            }
        }

        public double RelPositionTheta // degrees
        {
            get {return Math.Atan2 (RelPosition.Y, RelPosition.X) * 180 / Math.PI;}

            set
            {
                double rho   = RelPositionRho;
                double theta = value * Math.PI / 180;
                double phi   = RelPositionPhi * Math.PI / 180;
                RelPosition.X = rho * Math.Sin (phi) * Math.Cos (theta);
                RelPosition.Y = rho * Math.Sin (phi) * Math.Sin (theta); ;
                RelPosition.Z = rho * Math.Cos (phi);
                UpdateCamera ();
            }
        }

        public double RelPositionPhi // degrees
        {
            get
            {
                double XYLength = Math.Sqrt (RelPosition.X * RelPosition.X + RelPosition.Y * RelPosition.Y);
                return  Math.Atan2 (XYLength, RelPosition.Z) * 180 / Math.PI;  
            }

            set
            {
                double rho   = RelPositionRho;
                double theta = RelPositionTheta * Math.PI / 180;
                double phi   = value * Math.PI / 180;
                RelPosition.X = rho * Math.Sin (phi) * Math.Cos (theta);
                RelPosition.Y = rho * Math.Sin (phi) * Math.Sin (theta); ;
                RelPosition.Z = rho * Math.Cos (phi);
                UpdateCamera ();
            }
        }

        // Camera absolute position
        public Point3D AbsPosition
        {
            get {return ViewCenter + (Vector3D) RelPosition;}
            set {RelPosition = value - (Vector3D) ViewCenter; UpdateCamera ();}
        }


        private Point3D viewCenter = new Point3D (0, 0, 0);

        public Point3D ViewCenter
        {
            get {return viewCenter;}
            set {viewCenter = value; UpdateCamera ();}
        }





        //public double Distance
        //{
        //    get { return ((Vector3D)RelPosition).Length; }
        //    set { }
        //    //      set {RelPositionRho = value;}
        //}

        public Vector3D Direction {get {return (-1 * (Vector3D) RelPosition);}}    // ------------- needs setter?

        public Vector3D Right
        {
            get
            {
                SphericalCoordPoint right = new SphericalCoordPoint (1, RelPositionTheta + 90, 90);
                return (Vector3D) right.Cartesian;
            }
        }

        public Vector3D Up {get {return Vector3D.CrossProduct ((Vector3D) RelPosition, Right);}}

        //***************************************************************************************
        //
        // Lighting that moves with camera (like a headlamp)
        //
        Lights lighting = null; // optional, may remain null
       


        private void UpdateCamera ()
        {
            if (Camera != null)
            {
                Camera.Position      = AbsPosition;
                Camera.LookDirection = Direction;
                Camera.UpDirection   = Up;
            }

            if (lighting != null)
                lighting.Direction = Direction;
        }


        //**************************************************************************************

        public ProjectionCameraWrapper () : this (ProjectionType.Perspective)
        {
        }

        public ProjectionCameraWrapper (ProjectionType projection, Lights lts = null)
        {   
            if (projection == ProjectionType.Perspective)
                Camera = new PerspectiveCamera (AbsPosition, Direction, Up, FOV);   
            else
                Camera = new OrthographicCamera (AbsPosition, Direction, Up, Width);

            Camera.NearPlaneDistance = 0.1;
            Camera.FarPlaneDistance = 10000;

            UpdateCamera ();
            lighting = lts; 
        }



        //private void EL (string str)
        //{
        //    EventLog.WriteLine (str);
        //    EventLog.WriteLine (string.Format ("AbsPosition {0:0.00}", AbsPosition));
        //    EventLog.WriteLine (string.Format ("RelPosition {0:0.00}", RelPosition));
        //    EventLog.WriteLine (string.Format ("ViewCenter  {0:0.00}", ViewCenter));
        //}


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

        void ProcessNewPhi ()
        {
            RelPositionPhi = Phi;
            UpdateCamera ();
        }

        //*********************************************************************************************

        void ProcessNewTheta ()
        {
            RelPositionTheta = Theta;
            UpdateCamera ();
        }

        double fov = 45;   // field of view, degrees, for perspective camera
        double width = 7;  // for orthographic camera

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
    }
}
