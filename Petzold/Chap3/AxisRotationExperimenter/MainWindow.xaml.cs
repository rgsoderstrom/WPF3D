using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace AxisRotationExperimenter
{
    public partial class MainWindow : Window
    {
        //******************************************************************

        // initial positions and orientations
        const double InitialRho   = 15;
        const double InitialPhi   = 60;
        const double InitialTheta = 40;
        const double InitialFOV   = 45;
        const double InitialCenterX = 0;//2;
        const double InitialCenterY = 0;//1;
        const double InitialCenterZ = 0;

        //******************************************************************

        // Model Rotation3D Transforms
        AxisAngleRotation3D ModelX_AAR = new AxisAngleRotation3D (new Vector3D (1, 0, 0), 0); 
        AxisAngleRotation3D ModelY_AAR = new AxisAngleRotation3D (new Vector3D (0, 1, 0), 0); 
        AxisAngleRotation3D ModelZ_AAR = new AxisAngleRotation3D (new Vector3D (0, 0, 1), 0); 
         
        // Camera Transforms 
        AxisAngleRotation3D  Phi_AAR   = new AxisAngleRotation3D (new Vector3D (0, 1, 0), InitialPhi); 
        AxisAngleRotation3D  Theta_AAR = new AxisAngleRotation3D (new Vector3D (0, 0, 1), InitialTheta);
        AxisAngleRotation3D  Tilt_AAR  = new AxisAngleRotation3D (new Vector3D (0, 0, -1), 0);
        AxisAngleRotation3D  Pan_AAR   = new AxisAngleRotation3D (new Vector3D (-1, 0, 0), 0);
        TranslateTransform3D Translate = new TranslateTransform3D (InitialCenterX, InitialCenterY, InitialCenterZ); // translate camera "centered on" point

        RotateTransform3D PanRotate; // need access to this one later, to change "center" fields. Others always
                                     // centered on origin

        Transform3DGroup cameraXforms = new Transform3DGroup ();

        //******************************************************************

        public MainWindow ()
        {
            InitializeComponent ();
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            {
                Model3DGroup wedge = Wedge.MakeModel ();

                Transform3DGroup modelXforms = new Transform3DGroup ();
                modelXforms.Children.Add (new RotateTransform3D (ModelX_AAR));
                modelXforms.Children.Add (new RotateTransform3D (ModelY_AAR));
                modelXforms.Children.Add (new RotateTransform3D (ModelZ_AAR));
                wedge.Transform = modelXforms;

                //
                // Lights
                //
                AmbientLight ambient = new AmbientLight (Color.FromRgb (0x40, 0x40, 0x40));
                DirectionalLight directional = new DirectionalLight (Color.FromRgb (0xc0, 0xc0, 0xc0), new Vector3D (2, -3, -1));

                //
                // Visuals from Models
                //
                ModelVisual3D wedgeVisual = new ModelVisual3D ();
                wedgeVisual.Content = wedge;

                ModelVisual3D ambientVisual = new ModelVisual3D ();
                ambientVisual.Content = ambient;

                ModelVisual3D directionalVisual = new ModelVisual3D ();
                directionalVisual.Content = directional;

                //
                // Viewport
                //
                Viewport.Camera = new PerspectiveCamera (new Point3D (0, 0, 1 /*InitialRho*/), new Vector3D (0, 0, -1), new Vector3D (-1, 0, 0), InitialFOV);

                rhoSlider.Value = InitialRho;
                phiSlider.Value = Phi_AAR.Angle;
                thetaSlider.Value = Theta_AAR.Angle;
                centerX.Text = InitialCenterX.ToString ();
                centerY.Text = InitialCenterY.ToString ();
                centerZ.Text = InitialCenterZ.ToString ();

                PanRotate = new RotateTransform3D (Pan_AAR, new Point3D (0, 0, InitialRho));

                cameraXforms.Children.Add (new RotateTransform3D (Tilt_AAR)); 
                cameraXforms.Children.Add (PanRotate);
                cameraXforms.Children.Add (new RotateTransform3D (Phi_AAR)); 
                cameraXforms.Children.Add (new RotateTransform3D (Theta_AAR));
                cameraXforms.Children.Add (Translate);

                Viewport.Camera.Transform = cameraXforms;

                // directional light
                directional.Transform = cameraXforms;

                Viewport.Children.Add (wedgeVisual);
                Viewport.Children.Add (ambientVisual);
                Viewport.Children.Add (directionalVisual);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        //*********************************************************************************************

        private void UpdateCameraAbsPositionText ()
        {
            if (Viewport != null)
            {
                Point3D pos = (Viewport.Camera as ProjectionCamera).Position; // always along the Z axis, (0, 0, rho)

                Transform3DGroup xform = new Transform3DGroup ();
                xform.Children.Add (new RotateTransform3D (Phi_AAR));
                xform.Children.Add (new RotateTransform3D (Theta_AAR)); 

                Point3D relPosition = xform.Transform (pos);
                Point3D absPosition = relPosition + new Vector3D (Translate.OffsetX, Translate.OffsetY, Translate.OffsetZ);

                absPositionX.TextChanged -= absPosition_TextChanged;
                absPositionY.TextChanged -= absPosition_TextChanged;
                absPositionZ.TextChanged -= absPosition_TextChanged;

                absPositionX.Text = String.Format ("{0:0.0}", absPosition.X);
                absPositionY.Text = String.Format ("{0:0.0}", absPosition.Y);
                absPositionZ.Text = String.Format ("{0:0.0}", absPosition.Z);

                absPositionX.TextChanged += absPosition_TextChanged;
                absPositionY.TextChanged += absPosition_TextChanged;
                absPositionZ.TextChanged += absPosition_TextChanged;
            }
        }

        private void UpdateCameraRelPositionText ()
        {
            if (Viewport != null)
            {
                Point3D pos = (Viewport.Camera as ProjectionCamera).Position; // always along the Z axis, (0, 0, rho)

                Transform3DGroup xform = new Transform3DGroup ();
                xform.Children.Add (new RotateTransform3D (Phi_AAR));
                xform.Children.Add (new RotateTransform3D (Theta_AAR)); 

                Point3D relPosition = xform.Transform (pos);

                relPositionX.TextChanged -= relPosition_TextChanged;
                relPositionY.TextChanged -= relPosition_TextChanged;
                relPositionZ.TextChanged -= relPosition_TextChanged;

                relPositionX.Text = String.Format ("{0:0.0}", relPosition.X);
                relPositionY.Text = String.Format ("{0:0.0}", relPosition.Y);
                relPositionZ.Text = String.Format ("{0:0.0}", relPosition.Z);

                relPositionX.TextChanged += relPosition_TextChanged;
                relPositionY.TextChanged += relPosition_TextChanged;
                relPositionZ.TextChanged += relPosition_TextChanged;
            }
        }

        //*********************************************************************************************

        private void thetaSlider_ValueChanged (SliderTextAndLabel src, SliderAndTextEventArgs args)
        {
            Theta_AAR.Angle = args.SliderValue;
            UpdateCameraRelPositionText ();
            UpdateCameraAbsPositionText ();
        }

        private void phiSlider_ValueChanged (SliderTextAndLabel src, SliderAndTextEventArgs args)
        {
            Phi_AAR.Angle = args.SliderValue;
            UpdateCameraRelPositionText ();
            UpdateCameraAbsPositionText ();
        }

        private void rhoSlider_ValueChanged (SliderTextAndLabel src, SliderAndTextEventArgs args)
        {
            if (Viewport != null)
                (Viewport.Camera as ProjectionCamera).Position = new Point3D (0, 0, args.SliderValue);

            if (PanRotate != null)
                PanRotate.CenterZ = args.SliderValue;

            UpdateCameraRelPositionText ();
            UpdateCameraAbsPositionText ();
        }

        private void fovSlider_ValueChanged (SliderTextAndLabel src, SliderAndTextEventArgs args)
        {
            if (Viewport != null)
                if (Viewport.Camera is PerspectiveCamera)
                    (Viewport.Camera as PerspectiveCamera).FieldOfView = args.SliderValue;
        }

        private void panSlider_ValueChanged (SliderTextAndLabel src, SliderAndTextEventArgs args)
        {
            Pan_AAR.Angle = args.SliderValue;
        }

        private void tiltSlider_ValueChanged (SliderTextAndLabel src, SliderAndTextEventArgs args)
        {
            Tilt_AAR.Angle = args.SliderValue;
        }

        private void center_TextChanged (object sender, TextChangedEventArgs e)
        {
            try
            {
                Translate.OffsetX = double.Parse (centerX.Text);
                Translate.OffsetY = double.Parse (centerY.Text);
                Translate.OffsetZ = double.Parse (centerZ.Text);

                UpdateCameraRelPositionText ();
                UpdateCameraAbsPositionText ();
            }

            catch (Exception ex)
            {
            }
        }

        private void relPosition_TextChanged (object sender, TextChangedEventArgs e)
        {
            try
            {
                double x = double.Parse (relPositionX.Text);
                double y = double.Parse (relPositionY.Text);
                double z = double.Parse (relPositionZ.Text);

                PointSph pt = new PointSph (new Point3D (x, y, z));

                rhoSlider.ValueChanged   -= rhoSlider_ValueChanged;
                thetaSlider.ValueChanged -= thetaSlider_ValueChanged;
                phiSlider.ValueChanged   -= phiSlider_ValueChanged;

                PanRotate.CenterZ = rhoSlider.Value = pt.Rho;
                (Viewport.Camera as ProjectionCamera).Position = new Point3D (0, 0, pt.Rho);
                Theta_AAR.Angle = thetaSlider.Value = pt.Theta;
                Phi_AAR.Angle = phiSlider.Value = pt.Phi;

                rhoSlider.ValueChanged   += rhoSlider_ValueChanged;
                thetaSlider.ValueChanged += thetaSlider_ValueChanged;
                phiSlider.ValueChanged   += phiSlider_ValueChanged;

                UpdateCameraAbsPositionText ();
            }

            catch (Exception)
            {
            }
        }

        private void absPosition_TextChanged (object sender, TextChangedEventArgs e)
        {
            try
            {
                double x = double.Parse (absPositionX.Text);
                double y = double.Parse (absPositionY.Text);
                double z = double.Parse (absPositionZ.Text);

                Point3D ptAbs = new Point3D (x, y, z);
                Point3D ptRel = ptAbs - new Vector3D (Translate.OffsetX, Translate.OffsetY, Translate.OffsetZ);

                PointSph pt = new PointSph (ptRel);

                rhoSlider.ValueChanged   -= rhoSlider_ValueChanged;
                thetaSlider.ValueChanged -= thetaSlider_ValueChanged;
                phiSlider.ValueChanged   -= phiSlider_ValueChanged;

                PanRotate.CenterZ = rhoSlider.Value = pt.Rho;
                (Viewport.Camera as ProjectionCamera).Position = new Point3D (0, 0, pt.Rho);
                Theta_AAR.Angle = thetaSlider.Value = pt.Theta;
                Phi_AAR.Angle = phiSlider.Value = pt.Phi;

                rhoSlider.ValueChanged   += rhoSlider_ValueChanged;
                thetaSlider.ValueChanged += thetaSlider_ValueChanged;
                phiSlider.ValueChanged   += phiSlider_ValueChanged;

                UpdateCameraRelPositionText ();
            }

            catch (Exception)
            {
            }
        }
    }
}
