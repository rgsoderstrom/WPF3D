using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace WPF3D.MouseTracking
{
    public class VisualLookup  // retreive an object from its "Visual" property
    {
        Dictionary<ModelVisual3D, IMouseTracking> dict = new Dictionary<ModelVisual3D, IMouseTracking> ();

        public void Add (IMouseTracking obj)
        {
            dict [obj.Visual] = obj;
        }

        public void Remove (IMouseTracking obj)
        {
            dict.Remove (obj.Visual);
        }

        public IMouseTracking GetHookable (ModelVisual3D mod)
        {
            IMouseTracking obj = null;

            try
            {
                obj = dict [mod];
            }

            catch (KeyNotFoundException) // not an error
            {
            }

            return obj;
        }

        public VisualLookup ()
        {
        }
    }
}
