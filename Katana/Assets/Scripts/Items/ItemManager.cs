using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ItemManager : MonoBehaviour
    {
        private float _coins;
        public float Coins
        {
            get => _coins;

            private set
            {
                if (Mathf.Approximately(_coins, value)) return;
                _coins = value;
                OnCoinsChanged?.Invoke();
            }
        }
        public event Action OnCoinsChanged;
        
        public Item SelectedItem { get; private set; }
        
        public List<Item> Items;

        private void Awake()
        {
            var itemName = PlayerPrefs.GetString("SelectedItem");
            var item = Items.FirstOrDefault(x => x.Name == itemName);
            SelectedItem = item ?? Items[0];
            PlayerPrefs.SetString("SelectedItem", itemName);
        }
        
        public void SelectItem(Item item)
        {
            if (!item.Unlocked)
            {
                item.Unlocked =  true;
                Coins         -= item.Price;
            }
            
            SelectedItem = item;
            PlayerPrefs.SetString("SelectedItem", item.Name);
        }
    }
}