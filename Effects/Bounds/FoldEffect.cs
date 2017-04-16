using System;

namespace VisualEffects.Effects.Bounds
{
    public class FoldEffect : IEffect
    {
        public EffectInteractions Interaction
        {
            get { return EffectInteractions.BOUNDS; }
        }

        public int GetCurrentValue( System.Windows.Forms.Control control )
        {
            return control.Width * control.Height;
        }

        public void SetValue( System.Windows.Forms.Control control, int originalValue, int valueToReach, int newValue )
        {
            int actualValueChange = Math.Abs( originalValue - valueToReach );
            int currentValue = GetCurrentValue( control );

            double absoluteChangePerc = ( (double)( ( originalValue - newValue ) * 100 ) ) / actualValueChange;
            absoluteChangePerc = Math.Abs( absoluteChangePerc );

            if( absoluteChangePerc > 100.0f )
                return;
        }

        public int GetMinimumValue( System.Windows.Forms.Control control )
        {
            if( control.MinimumSize.IsEmpty )
                return 0;

            return control.MinimumSize.Width * control.MinimumSize.Height;
        }

        public int GetMaximumValue( System.Windows.Forms.Control control )
        {
            if( control.MaximumSize.IsEmpty )
                return Int32.MaxValue;

            return control.MaximumSize.Width * control.MaximumSize.Height;
        }
    }
}
