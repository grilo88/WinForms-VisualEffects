using System;
using System.Drawing;
using System.Windows.Forms;

namespace VisualEffects.Effects.Bounds
{
    public class RightAnchoredWidthEffect : IEffect
    {
        public int GetCurrentValue( Control control )
        {
            return control.Width;
        }

        public void SetValue( Control control, int originalValue, int valueToReach, int newValue )
        {
            //changing location and size independently can cause flickering:
            //change bounds property instead.

            var size = new Size( newValue, control.Height );
            var location = new Point( control.Left +
                ( control.Width - newValue ), control.Top );

            control.Bounds = new Rectangle( location, size );
        }

        public int GetMinimumValue( Control control )
        {
            if( control.MinimumSize.IsEmpty )
                return Int32.MinValue;

            return control.MinimumSize.Width;
        }

        public int GetMaximumValue( Control control )
        {
            if( control.MaximumSize.IsEmpty )
                return Int32.MaxValue;

            return control.MaximumSize.Width;
        }

        public EffectInteractions Interaction
        {
            get { return EffectInteractions.BOUNDS; }
        }
    }
}
