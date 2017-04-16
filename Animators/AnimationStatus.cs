using System;
using System.Diagnostics;
using System.Threading;
using VisualEffects.Effects;

namespace VisualEffects.Animators
{
    public class AnimationStatus : EventArgs
    {
        private readonly Stopwatch _stopwatch;

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

        public CancellationTokenSource CancellationToken { get; private set; }
        public bool IsCompleted { get; set; }

        public AnimationStatus( CancellationTokenSource token, Stopwatch stopwatch )
        {
            CancellationToken = token;
            _stopwatch = stopwatch;
        }
        public IEffect Effect { get; set; }
    }
}
