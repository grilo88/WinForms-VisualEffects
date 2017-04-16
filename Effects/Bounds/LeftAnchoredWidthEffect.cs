using System;
using System.Windows.Forms;

namespace VisualEffects.Effects.Bounds
{
    public class LeftAnchoredWidthEffect : IEffect
    {
        public int GetCurrentValue( Control control )
        {
            return control.Width;
        }

        public void SetValue( Control control, int originalValue, int valueToReach, int newValue )
        {
            control.Width = newValue;
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
            get { return EffectInteractions.WIDTH; }
        }
    }
}
