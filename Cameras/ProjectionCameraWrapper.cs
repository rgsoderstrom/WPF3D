
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

                PanRotate.CenterZ = pt.Rho;
                Camera.Position = new Point3D (0, 0, pt.Rho);
                Theta_AAR.Angle = pt.Theta;
                Phi_AAR.Angle   = pt.Phi;
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
                PanRotate.CenterZ = pt.Rho;
                Camera.Position = new Point3D (0, 0, pt.Rho);
                Theta_AAR.Angle = pt.Theta;
                Phi_AAR.Angle = pt.Phi;
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
