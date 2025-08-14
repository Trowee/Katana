using System;
using UnityEngine;

namespace Assets.Scripts.Modifiers
{
    public class ModifierItem
    {
        public Modifier Modifier;

        private float _elapsedTime;
        public float ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = Mathf.Clamp(value, 0, Modifier.Duration);
                if (_elapsedTime >= Modifier.Duration) Expire();
            }
        }

        public bool Expired;
        public Action OnExpired;

        public ModifierItem(Modifier modifier) => Modifier = modifier;

        public void Expire()
        {
            Expired = true;
            OnExpired?.Invoke();
        }
    }
}
