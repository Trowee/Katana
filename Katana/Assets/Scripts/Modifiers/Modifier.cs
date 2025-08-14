using UnityEngine;

namespace Assets.Scripts.Modifiers
{
    public class Modifier : ScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public bool Stackable;
        public bool ReplaceExisting;
        public bool UseDuration;
        public float Duration;
        public bool Scaled;
    }
}
