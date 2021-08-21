using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Diagnostics; // stopwatch

using WPF3D.Lighting;
using WPF3D.Cameras;

namespace WPF3D.MouseTracking
{
    public class MouseTracking : DependencyObject
    {
      // private 
        bool   mousePointerOverAVisual {get {return Highlighted != null;}}
        bool   mouseTrackingActive = false;
        Point  mouseOrigin;
        double originTheta;
        double originPhi;
        double wpfToTheta {get {return 360 / view.ActualWidth;}}
        double wpfToPhi   {get {return 180 / view.ActualHeight;}}

      // public interface properties
        public VisualLookup Dictionary {get; private set;} = new VisualLookup ();

        IMouseTracking Highlighted {get; set;} = null;
        List<IMouseTracking> Selected = new List<IMouseTracking> ();

        public IMouseTracking ModifyTarget {get {if (Selected.Count == 1) return Selected [0]; else return null;}}

      // copies of references in client window
        Viewport3D view; 
        ProjectionCameraWrapper camera;

        //***********************************************************************************************************

        //
        // for data binding in main window
        //

        public static readonly DependencyProperty AllowGroupingProperty = DependencyProperty.Register("AllowGrouping", typeof(bool), 
                                                                                                      typeof(MouseTracking),                
                                                                                                      new PropertyMetadata (false, PropertyChanged));
        bool AllowGrouping
        {
            set { SetValue(AllowGroupingProperty, value); }
            get { return (bool)GetValue(AllowGroupingProperty); }
        }

        public static readonly DependencyProperty AllowUngroupingProperty = DependencyProperty.Register("AllowUngrouping", typeof(bool), 
                                                                                                         typeof(MouseTracking),                
                                                                                                         new PropertyMetadata (false, PropertyChanged));
        bool AllowUngrouping
        {
            set { SetValue(AllowUngroupingProperty, value); }
            get { return (bool)GetValue(AllowUngroupingProperty); }
        }

        private static void PropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MouseTracking).PropertyChanged (e);
        }

        private void PropertyChanged (DependencyPropertyChangedEventArgs args)
        {
            if (args.Property == AllowGroupingProperty)
            {
            }

            if (args.Property == AllowUngroupingProperty)
            {
            }
        }

        //***********************************************************************************************************

        public MouseTracking (Viewport3D v, ProjectionCameraWrapper c)
        {
            view = v;
            camera = c;
        }

        //***********************************************************************************************************

        // return a copy of Selected list so application can process

        public List<IMouseTracking> CloneAndClearSelected ()
        {
            List<IMouseTracking> listCopy = new List<IMouseTracking> ();
            listCopy.AddRange (Selected);

            foreach (IMouseTracking imt in Selected)
                imt.SelectedHighlight = false;

            Selected.Clear ();
            return listCopy;
        }

        public void AddSelected (IMouseTracking imt)
        {
            imt.SelectedHighlight = true;
            Selected.Add (imt);

            AllowGrouping   = Selected.Count > 1;
            AllowUngrouping = Selected.Count == 1 && Selected [0].CanUngroup;
        }

        public void SetSelected (IMouseTracking imt)
        {
            Selected.Clear ();
            imt.SelectedHighlight = true;
            Selected.Add (imt);

            AllowGrouping   = false;
            AllowUngrouping = imt.CanUngroup;
        }

        //***********************************************************************************************************

        private void OnMouseClicked ()
        {
            if (mousePointerOverAVisual == false)
            {
                foreach (IMouseTracking im in Selected)
                    im.SelectedHighlight = false;

                Selected.Clear ();

                AllowGrouping   = false;
                AllowUngrouping = false;
            }
        }

        //***********************************************************************************************************

        private Stopwatch stopwatch = new Stopwatch ();

        public void OnMouseUp (MouseButtonEventArgs args)
        {
            mouseTrackingActive = false;
            view.ReleaseMouseCapture ();

            if (stopwatch.ElapsedMilliseconds < 200)
                OnMouseClicked ();
        }

        //***********************************************************************************************************

        public void OnMouseDown (MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Left)
            {
                stopwatch.Reset ();
                stopwatch.Start ();

                Point pt = args.GetPosition (view);

                bool inViewport = pt.X > 0 && pt.Y > 0 && pt.X < view.ActualWidth && pt.Y < view.ActualHeight;

                if (inViewport == false)
                    return;

                if (mousePointerOverAVisual)
                {
                    if (Keyboard.IsKeyDown (Key.LeftShift) || Keyboard.IsKeyDown (Key.RightShift))
                    {
                        Selected.Add (Highlighted);
                        Highlighted.SelectedHighlight = true;
                    }
                    else
                    {
                        foreach (IMouseTracking im in Selected)
                            im.SelectedHighlight = false;

                        Selected.Clear ();
                        Selected.Add (Highlighted);
                        Highlighted.SelectedHighlight = true;
                    }

                    AllowGrouping   = (Selected.Count > 1);
                    AllowUngrouping = (Selected.Count == 1) && Selected [0].CanUngroup;
                }
                else
                {
                    mouseTrackingActive = true;
                    mouseOrigin = pt;
                    originPhi = camera.Phi;
                    originTheta = camera.Theta;
                    view.CaptureMouse ();
                }
            }
        }

        //***********************************************************************************************************

        public void OnMouseMove (MouseEventArgs args)
        {
            Point pt = args.GetPosition (view);

            if (mouseTrackingActive)
            {
                double dx = pt.X - mouseOrigin.X;
                double dy = pt.Y - mouseOrigin.Y;

                double phi = originPhi - dy * wpfToPhi;
                double theta = originTheta - dx * wpfToTheta;

                while (phi < 0) phi += 360;
                while (phi > 360) phi -= 360;

                while (theta < 0) theta += 360;
                while (theta > 360) theta -= 360;

                camera.Phi   = phi;
                camera.Theta = theta;

                return;
            }
            
            if (Highlighted != null)
            {
                Highlighted.MouseOverHighlight = false;
                Highlighted = null;
            }

            bool inWindow = pt.X > 0 && pt.Y > 0 && pt.X < view.ActualWidth && pt.Y < view.ActualHeight;

            if (inWindow == false)
                return;

            // get the objects under the mouse pointer
            HitTestResult result = VisualTreeHelper.HitTest (view, pt);

            if (result == null)
                return;

  //        Console.WriteLine ("HitTestResult {0}", result.ToString ());
  
            // cast the result parameter
            RayMeshGeometry3DHitTestResult resultMesh = result as RayMeshGeometry3DHitTestResult;

            if (resultMesh == null)
                return;

   //      Console.WriteLine ("RayMeshGeometry3DHitTestResult {0}", resultMesh.ModelHit);
  
            // get the ModelVisual that was clicked
            ModelVisual3D vis = resultMesh.VisualHit as ModelVisual3D;

            if (vis == null)
                return;

   //       Console.WriteLine ("Visual {0}", vis.ToString ());

            IMouseTracking obj = Dictionary.GetHookable (vis);

            if (obj == null)
            {
   //           Console.WriteLine ("Hookable null");
                return;
            }

   //       Console.WriteLine ("iMouseTracking {0}", obj.Name);

            Highlighted = obj;
            Highlighted.MouseOverHighlight = true; 
        }
    }
}
