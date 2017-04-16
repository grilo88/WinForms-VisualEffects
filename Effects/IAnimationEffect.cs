using System.Windows.Forms;

namespace VisualEffects.Effects
{
    /// <summary>
    /// By implementing this interface you define what property of your control
    /// is manipulated and the way you manipulate it.
    /// </summary>
    public interface IEffect
    {
        EffectInteractions Interaction { get; }

        int GetCurrentValue( Control control );
        void SetValue( Control control, int originalValue, int valueToReach, int newValue );

        int GetMinimumValue( Control control );
        int GetMaximumValue( Control control );
    }
}
