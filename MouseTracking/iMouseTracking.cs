using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPF3D.MouseTracking
{
    public interface IMouseTracking
    {
        ModelVisual3D Visual {get;}
        Model3D       Model  {get;} // base class of what is actually returned: GeometryModel3D or Model3DGroup

        bool MouseOverHighlight {set;}
        bool SelectedHighlight {set;} 

        bool CanUngroup {get;}
    }
}
