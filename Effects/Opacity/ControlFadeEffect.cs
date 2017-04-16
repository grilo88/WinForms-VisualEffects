using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using VisualEffects.Extensions;

namespace VisualEffects.Effects.Opacity
{
    public class ControlFadeEffect : IEffect
    {
        public const int MaxOpacity = 100;
        public const int MinOpacity = 0;

        private class State
        {
            public int Opacity { get; set; }
            public Graphics ParentGraphics { get; set; }
            public Rectangle PreviousBounds { get; set; }
            public Bitmap Snapshot { get; set; }
        }

        private static Dictionary<Control, State> _controlCache
            = new Dictionary<Control, State>();

        public ControlFadeEffect( Control control )
        {
            if( !_controlCache.ContainsKey( control ) )
            {
                var parentGraphics = control.Parent.CreateGraphics();
                parentGraphics.CompositingQuality = CompositingQuality.HighSpeed;

                var state = new State()
                {
                    ParentGraphics = parentGraphics,
                    Opacity = control.Visible ? MaxOpacity : MinOpacity,
                };

                _controlCache.Add( control, state );
            }
        }

        public int GetCurrentValue( Control control )
        {
            return _controlCache[ control ].Opacity;
        }

        public void SetValue( Control control, int originalValue, int valueToReach, int newValue )
        {
            var state = _controlCache[ control ];

            //invalidate region no more in use
            var region = new Region( state.PreviousBounds );
            region.Exclude( control.Bounds );
            control.Parent.Invalidate( region );

            //I get real-time snapshot (no cache) so i can mix animations
            var snapshot = control.GetSnapshot();
            if( snapshot != null )
            {
                snapshot = (Bitmap)snapshot.ChangeOpacity( newValue );
                //avoid refresh and thus flickering: blend parent's background with snapshot
                var bgBlendedSnapshot = BlendWithBgColor( snapshot, control.Parent.BackColor );
                state.Snapshot = bgBlendedSnapshot;
            }
            state.PreviousBounds = control.Bounds;

            if( newValue == MaxOpacity )
            {
                control.Visible = true;
                return;
            }

            control.Visible = false;
            state.Opacity = newValue;

            if( newValue > 0 )
            {
                var rect = control.Parent.RectangleToClient(
                    control.RectangleToScreen( control.ClientRectangle ) );

                if( state.Snapshot != null )
                    state.ParentGraphics.DrawImage( state.Snapshot, rect );
            }
            else
            {
                control.Parent.Invalidate();
            }
        }

        public int GetMinimumValue( Control control )
        {
            return MinOpacity;
        }

        public int GetMaximumValue( Control control )
        {
            return MaxOpacity;
        }

        public EffectInteractions Interaction
        {
            get { return EffectInteractions.TRANSPARENCY; }
        }

        private Bitmap BlendWithBgColor( Image image1, System.Drawing.Color bgColor )
        {
            var finalImage = new Bitmap( image1.Width, image1.Height );
            using( Graphics g = Graphics.FromImage( finalImage ) )
            {
                g.Clear( System.Drawing.Color.Black );

                g.FillRectangle( new SolidBrush( bgColor ), new Rectangle( 0, 0, image1.Width, image1.Height ) );
                g.DrawImage( image1, new Rectangle( 0, 0, image1.Width, image1.Height ) );
            }

            return finalImage;
        }
    }

    /// <summary>
    /// tries to merge form snapshot and control snapshot
    /// </summary>
    public class ControlFadeEffect2 : IEffect
    {
        public const int MaxOpacity = 100;
        public const int MinOpacity = 0;

        private class State
        {
            public int Opacity { get; set; }
            public Graphics ParentGraphics { get; set; }
            public Rectangle PreviousBounds { get; set; }
            public Bitmap Snapshot { get; set; }
        }

        private static readonly Dictionary<Control, State> ControlCache
            = new Dictionary<Control, State>();

        public ControlFadeEffect2( Control control )
        {
            if (ControlCache.ContainsKey(control)) return;
            var parentGraphics = control.Parent.CreateGraphics();
            parentGraphics.CompositingQuality = CompositingQuality.HighSpeed;

            var state = new State
            {
                ParentGraphics = parentGraphics,
                Opacity = control.Visible ? MaxOpacity : MinOpacity,
            };

            ControlCache.Add( control, state );
        }

        public int GetCurrentValue( Control control )
        {
            return ControlCache[ control ].Opacity;
        }

        public void SetValue( Control control, int originalValue, int valueToReach, int newValue )
        {
            var state = ControlCache[ control ];

            //invalidate region no more in use
            var region = new Region( state.PreviousBounds );
            region.Exclude( control.Bounds );
            control.Parent.Invalidate( region );

            var form = control.FindForm();
            if(null == form) return;
            var formRelativeCoords = form.RectangleToClient( control.RectangleToScreen( control.ClientRectangle ) );

            //I get real-time snapshot (no cache) so i can mix animations
            var controlSnapshot = control.GetSnapshot();
            if( controlSnapshot != null )
            {
                controlSnapshot = (Bitmap)controlSnapshot.ChangeOpacity( newValue );


                var formSnapshot = form.GetFormBorderlessSnapshot().Clone( formRelativeCoords, PixelFormat.DontCare );

                //avoid refresh and thus flickering: blend parent form snapshot with control snapshot
                var bgBlendedSnapshot = BlendImages( formSnapshot, controlSnapshot );
                //bgBlendedSnapshot.Save( @"C:\Users\Sampietro.Mauro\Documents\_root\bmp" + newValue + ".bmp" );
                state.Snapshot = bgBlendedSnapshot;
            }
            state.PreviousBounds = control.Bounds;

            if( newValue == MaxOpacity )
            {
                control.Visible = true;
                return;
            }

            control.Visible = false;
            state.Opacity = newValue;

            if( newValue > 0 )
            {
                //var rect = control.Parent.RectangleToClient(
                //    control.RectangleToScreen( control.ClientRectangle ) );

                form.CreateGraphics().DrawImage( state.Snapshot, formRelativeCoords );
                //if( state.Snapshot != null )
                //    state.ParentGraphics.DrawImage( state.Snapshot, rect );
            }
            else
            {
                control.Parent.Invalidate();
            }
        }

        public int GetMinimumValue( Control control )
        {
            return MinOpacity;
        }

        public int GetMaximumValue( Control control )
        {
            return MaxOpacity;
        }

        public EffectInteractions Interaction
        {
            get { return EffectInteractions.TRANSPARENCY; }
        }

        private Bitmap BlendImages( Image image1, Image image2 )
        {
            var finalImage = new Bitmap( image1.Width, image1.Height );
            using( Graphics g = Graphics.FromImage( finalImage ) )
            {
                g.Clear( System.Drawing.Color.Black );

                g.DrawImage( image1, new Rectangle( 0, 0, image1.Width, image1.Height ) );
                g.DrawImage( image2, new Rectangle( 0, 0, image1.Width, image1.Height ) );
            }

            return finalImage;
        }
    }
}
