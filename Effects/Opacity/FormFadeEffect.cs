using System;
using System.Windows.Forms;

namespace VisualEffects.Effects.Opacity
{
    public class FormFadeEffect : IEffect
    {
        public int GetCurrentValue( Control control )
        {
            if( !( control is Form ) )
                throw new Exception( "Fading effect can be applied only on forms" );

            var form = (Form)control;
            return (int)( form.Opacity * 100 );
        }

        public void SetValue( Control control, int originalValue, int valueToReach, int newValue )
        {
            if( !( control is Form ) )
                throw new Exception( "Fading effect can be applied only on forms" );

            var form = (Form)control;
            form.Opacity = (float)newValue / 100;
        }

        public int GetMinimumValue( Control control )
        {
            return 0;
        }

        public int GetMaximumValue( Control control )
        {
            return 100;
        }

        public EffectInteractions Interaction => EffectInteractions.TRANSPARENCY;
    }
}
