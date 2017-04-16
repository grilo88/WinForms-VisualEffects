using System;
using System.Windows.Forms;

namespace VisualEffects.Effects.Color
{
    public class ColorShiftEffect : IEffect
    {
        public EffectInteractions Interaction => EffectInteractions.COLOR;

        public int GetCurrentValue( Control control )
        {
            return control.BackColor.ToArgb();
        }

        public void SetValue( Control control, int originalValue, int valueToReach, int newValue )
        {
            int actualValueChange = Math.Abs( originalValue - valueToReach );
            int currentValue = GetCurrentValue( control );

            double absoluteChangePerc = ( (double)( ( originalValue - newValue ) * 100 ) ) / actualValueChange;
            absoluteChangePerc = Math.Abs( absoluteChangePerc );

            if( absoluteChangePerc > 100.0f )
                return;

            System.Drawing.Color originalColor = System.Drawing.Color.FromArgb( originalValue );
            System.Drawing.Color newColor = System.Drawing.Color.FromArgb( valueToReach );

            int newA = Interpolate( originalColor.A, newColor.A, absoluteChangePerc );
            int newR = Interpolate( originalColor.R, newColor.R, absoluteChangePerc );
            int newG = Interpolate( originalColor.G, newColor.G, absoluteChangePerc );
            int newB = Interpolate( originalColor.B, newColor.B, absoluteChangePerc );

            control.BackColor = System.Drawing.Color.FromArgb( newA, newR, newG, newB );
        }

        public int GetMinimumValue( Control control )
        {
            return System.Drawing.Color.Black.ToArgb();
        }

        public int GetMaximumValue( Control control )
        {
            return System.Drawing.Color.White.ToArgb();
        }

        private int Interpolate( int val1, int val2, double changePerc )
        {
            int difference = val2 - val1;
            int distance = (int)( difference * ( changePerc / 100 ) );
            return val1 + distance;
        }
    }
}
