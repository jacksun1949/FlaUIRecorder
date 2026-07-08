using FlaUI.Core.AutomationElements.Infrastructure;
using System;

namespace FlaUIRecorder.Internal.Worker
{
    /// <summary>
    /// Shares the most recent hover lookup between workers to avoid redundant FromPoint calls.
    /// </summary>
    public class HoverElementCache
    {
        private readonly object _lock = new object();
        private AutomationElement _element;
        private int _x;
        private int _y;
        private DateTime _timestamp;

        public const int ReuseDistancePixels = 8;
        public const int FallbackDistancePixels = 32;
        public static readonly TimeSpan ReuseMaxAge = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Serializes UIA FromPoint calls across workers.
        /// </summary>
        public object SyncRoot => _lock;

        public void Update(int x, int y, AutomationElement element)
        {
            if (element == null)
                return;

            lock (_lock)
            {
                _x = x;
                _y = y;
                _element = element;
                _timestamp = DateTime.UtcNow;
            }
        }

        public bool TryGetNear(int x, int y, out AutomationElement element)
        {
            return TryGetWithin(x, y, ReuseDistancePixels, out element);
        }

        /// <summary>
        /// Returns a cached element when the click is within the given distance.
        /// Used as a wider fallback when a fresh FromPoint lookup fails.
        /// </summary>
        public bool TryGetWithin(int x, int y, int maxDistancePixels, out AutomationElement element)
        {
            lock (_lock)
            {
                element = null;
                if (_element == null)
                    return false;

                if (DateTime.UtcNow - _timestamp > ReuseMaxAge)
                    return false;

                var dx = x - _x;
                var dy = y - _y;
                if (dx * dx + dy * dy <= maxDistancePixels * maxDistancePixels)
                {
                    element = _element;
                    return true;
                }

                return false;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _element = null;
            }
        }
    }
}
