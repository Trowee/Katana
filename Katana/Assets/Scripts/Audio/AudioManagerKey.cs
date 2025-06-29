using System;

namespace Assets.Scripts.Audio
{
    public struct AudioManagerKey : IEquatable<AudioManagerKey>
    {
        public SourceType SourceType;
        public string Name;

        public AudioManagerKey(SourceType sourceType, string name)
        {
            SourceType = sourceType;
            Name = name;
        }

        public bool Equals(AudioManagerKey other) =>
            SourceType == other.SourceType && Name == other.Name;

        public override bool Equals(object obj) => obj is AudioManagerKey other && Equals(other);

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + SourceType.GetHashCode();
            hash = hash * 23 + (Name?.GetHashCode() ?? 0);
            return hash;
        }
    }
}