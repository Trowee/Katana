using System;
using System.Collections.Generic;

namespace TimeScale
{
    [Serializable]
    public struct TimeScaleKeys
    {
        public List<TimeScaleKey> Keys;

        public TimeScaleKeys(TimeScaleKey key)
        {
            Keys = new() { key };
        }
        
        public TimeScaleKeys(List<TimeScaleKey> keys)
        {
            Keys = keys;
        }
    }
}
