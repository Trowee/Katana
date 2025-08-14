using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Modifiers
{
    public class ModifierManager : MonoBehaviour
    {
        public readonly List<ModifierItem> Modifiers = new();
        public Action<ModifierItem> OnModifierAdded;
        public Action<ModifierItem> OnModifierRemoved;
        public Func<bool> IsPaused;

        private void Update()
        {
            if (IsPaused != null && IsPaused()) return;

            foreach (var item in Modifiers.Where(x => x.Modifier.UseDuration))
                item.ElapsedTime += item.Modifier.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;
        }

        public bool AddModifier(Modifier modifier, out ModifierItem item)
        {
            item = Modifiers.First(x => x.Modifier == modifier);
            var exists = item != null;

            if (exists)
            {
                if (!modifier.Stackable)
                {
                    if (modifier.ReplaceExisting) item.Expire();
                    else return false;
                }
            }

            if (!exists)
            {
                // Have to store in i cuz you can't pass out to lambda
                var i = item = new(modifier);
                item.OnExpired += () => RemoveModifier(i);
            }

            Modifiers.Add(item);
            OnModifierAdded?.Invoke(item);
            return true;
        }

        public void RemoveModifier(ModifierItem modifierItem)
        {
            Modifiers.Remove(modifierItem);
            if (!modifierItem.Expired) modifierItem.Expire();
            OnModifierRemoved?.Invoke(modifierItem);
        }
    }
}
