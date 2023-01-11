
using System;
using System.Windows;  // for DependencyObject
using System.Windows.Media.Media3D; // need PresentationFramework reference
using System.Windows.Input; // key event args

using Common;
using CommonMath;
using WPF3D.Lighting;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF3D.Cameras
{
    public class ProjectionCameraWrapper : DependencyObject
    {
        public ProjectionCamera Camera {get; protected set;} // this will be set to PerspectiveCamera or OrthographicCamera in ctor

        // default position and orientation
        const double InitialRho   = 1;
        const double InitialPhi   = 60;
        const double InitialTheta = 40;
        const double InitialFOV   = 45;  // for perspective camera
        const double InitialWidth = 20;  // for orthographic camera
        const double InitialCenterX = 0;
        const double InitialCenterY = 0;
        const double InitialCenterZ = 0;

        // Camera Transforms 
        AxisAngleRotation3D  Phi_AAR   = new AxisAngleRotation3D (new Vector3D (0, 1, 0), InitialPhi); 
        AxisAngleRotation3D  Theta_AAR = new AxisAngleRotation3D (new Vector3D (0, 0, 1), InitialTheta);
        AxisAngleRotation3D  Tilt_AAR  = new AxisAngleRotation3D (new Vector3D (0, 0, -1), 0);
        AxisAngleRotation3D  Pan_AAR   = new AxisAngleRotation3D (new Vector3D (-1, 0, 0), 0);
        TranslateTransform3D Translate = new TranslateTransform3D (InitialCenterX, InitialCenterY, InitialCenterZ); // translate camera "centered on" point

        RotateTransform3D PanRotate; // need access to this one later, to change "center" fields. Others always
                                     // centered on origin

        Transform3DGroup cameraXforms = new Transform3DGroup ();

        //***********************************************************************************************************

        public ProjectionCameraWrapper (ProjectionType projection)
        {   
            if (projection == ProjectionType.Perspective) Camera = new PerspectiveCamera  (new Point3D (0, 0, InitialRho), new Vector3D (0, 0, -1), new Vector3D (-1, 0, 0), InitialFOV);
            else                                          Camera = new OrthographicCamera (new Point3D (0, 0, InitialRho), new Vector3D (0, 0, -1), new Vector3D (-1, 0, 0), InitialWidth);

            PanRotate = new RotateTransform3D (Pan_AAR, new Point3D (0, 0, InitialRho));

            cameraXforms.Children.Add (new RotateTransform3D (Tilt_AAR)); 
            cameraXforms.Children.Add (PanRotate);
            cameraXforms.Children.Add (new RotateTransform3D (Phi_AAR)); 
            cameraXforms.Children.Add (new RotateTransform3D (Theta_AAR));
            cameraXforms.Children.Add (Translate);

            Camera.Transform = cameraXforms;
        }

        //**************************************************************************************

        // dependency properties

        public static readonly DependencyProperty PhiProperty = DependencyProperty.Register("Phi", typeof(double), typeof(ProjectionCameraWrapper),                
                                                                                            new PropertyMetadata(InitialPhi, PropertyChanged));
        public double Phi
        {
            set {SetValue(PhiProperty, value);}
            get {return (double)GetValue(PhiProperty);}
        }

        public static readonly DependencyProperty ThetaProperty = DependencyProperty.Register("Theta", typeof(double), typeof(ProjectionCameraWrapper),                
                                                                                            new PropertyMetadata(InitialTheta, PropertyChanged));
        public double Theta
        {
            set {SetValue(ThetaProperty, value);}
            get {return (double)GetValue(ThetaProperty);}
        }

        public static readonly DependencyProperty RhoProperty = DependencyProperty.Register ("Rho", typeof (double), typeof (ProjectionCameraWrapper),
                                                                                            new PropertyMetadata (InitialRho, PropertyChanged));
        public double Rho
        {
            set { SetValue (RhoProperty, value); }
            get { return (double)GetValue (RhoProperty); }
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
            if (args.Property == RhoProperty)   ProcessNewRho ();
        }

        void ProcessNewPhi ()
        {
            Phi_AAR.Angle = Phi;
        }

        void ProcessNewTheta ()
        {
            Theta_AAR.Angle = Theta;
        }

        void ProcessNewRho ()
        {
            Camera.Position = new Point3D (0, 0, Rho);
            PanRotate.CenterZ = Rho;
        }

        //*********************************************************************************

        public double FOV
        {
            get {if (Camera is PerspectiveCamera) return (Camera as PerspectiveCamera).FieldOfView;  else throw new Exception ("Not a Perspective Camera"); }
            set {if (Camera is PerspectiveCamera) (Camera as PerspectiveCamera).FieldOfView = value; else throw new Exception ("Not a Perspective Camera"); }
        }

        public double Width
        {
            get {if (Camera is OrthographicCamera) return (Camera as OrthographicCamera).Width;  else throw new Exception ("Not an Orthographic Camera"); }
            set {if (Camera is OrthographicCamera) (Camera as OrthographicCamera).Width = value; else throw new Exception ("Not an Orthographic Camera"); }
        }

        //*********************************************************************************

        // Up and Right ignore the tilt & pan angles

        public Vector3D Up
        {
            get
            {
                Vector3D InitialUp = new Vector3D (-1, 0, 0);
                Transform3DGroup xform = new Transform3DGroup ();
                xform.Children.Add (new RotateTransform3D (Phi_AAR));
                xform.Children.Add (new RotateTransform3D (Theta_AAR)); 

                return xform.Transform (InitialUp);
            }
        }

        public Vector3D Right
        {
            get
            {
                Vector3D InitialRight = new Vector3D (0, 1, 0);
                Transform3DGroup xform = new Transform3DGroup ();
                xform.Children.Add (new RotateTransform3D (Phi_AAR));
                xform.Children.Add (new RotateTransform3D (Theta_AAR)); 

                return xform.Transform (InitialRight);
            }
        }

        //*********************************************************************************

        public Point3D AbsPosition
        {
            get 
            { 
                Point3D pos = Camera.Position; // always along the Z axis, (0, 0, rho)
                Transform3DGroup xform = new Transform3DGroup ();
                xform.Children.Add (new RotateTransform3D (Phi_AAR));
                xform.Children.Add (new RotateTransform3D (Theta_AAR)); 

                Point3D relPosition = xform.Transform (pos);
                Point3D absPosition = relPosition + new Vector3D (Translate.OffsetX, Translate.OffsetY, Translate.OffsetZ);
                return absPosition;
            }

            set
            {
                Point3D ptAbs = value;
                Point3D ptRel = ptAbs - new Vector3D (Translate.OffsetX, Translate.OffsetY, Translate.OffsetZ);

                PointSph pt = new PointSph (ptRel);

                Rho = pt.Rho;
                Theta = pt.Theta;
                Phi = pt.Phi;
            }
        }

        public Point3D RelPosition
        {
            get 
            {
                Point3D pos = Camera.Position; // always along the Z axis, (0, 0, rho)
                Transform3DGroup xform = new Transform3DGroup ();
                xform.Children.Add (new RotateTransform3D (Phi_AAR));
                xform.Children.Add (new RotateTransform3D (Theta_AAR)); 
                Point3D relPosition = xform.Transform (pos);
                return relPosition;
            }

            set
            {
                PointSph pt = new PointSph (value);

                Rho = pt.Rho;
                Theta = pt.Theta;
                Phi = pt.Phi;
            }
        }

        //*********************************************************************************

        public Point3D CenterOn
        {
            get
            {
                return new Point3D (Translate.OffsetX, Translate.OffsetY, Translate.OffsetZ);
            }

            set
            {
                Translate.OffsetX = value.X;
                Translate.OffsetY = value.Y;
                Translate.OffsetZ = value.Z;
            }
        }
    }
}
