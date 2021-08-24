using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WKC
{
    [Serializable]
    public class StoreItem : IDisposable
    {
        public Sprite Icon;
        [HideInInspector]
        public Transform UI;
        public string Name;
        public string BuyID;
        public string Price;
        public string Description;
        //通常是打折后新商品价格
        public string AlternativePrice;
        //通常是打折后新商品ID
        public string AlternativeBuyID;
        //配置表：多个商品字符串格式
        public string RewardsStringFormat;
#if IAP
        //商店SDK类型接口
        public UnityEngine.Purchasing.ProductType ProductType;
#endif
        public Dictionary<short, ushort> Rewards = new Dictionary<short, ushort>();

        public event Action<string, bool> AddStoreItemEventHandler;
        public bool IsBought
        {
            get { return PlayerPrefs.GetInt("ShopItem_" + BuyID, 0) == 1; }
            set { PlayerPrefs.SetInt("ShopItem_" + BuyID, (value ? 1 : 0)); }
        }

        public StoreItem(string buyID)
        {
            BuyID = buyID;
            if (IsBought) AddStoreItemEventHandler?.Invoke(buyID, true);
        }

        public StoreItem(StoreItem si)
        {
            BuyID = si.BuyID;
            Description = si.Description;
            Name = si.Name;
            Price = si.Price;
            Icon = si.Icon;
            AlternativeBuyID = si.AlternativeBuyID;
            AlternativePrice = si.AlternativePrice;

            if (!string.IsNullOrEmpty(si.RewardsStringFormat))
            {
                si.RewardsStringFormat.Split('|').ToList().ForEach(e =>
                {
                    var result = e.Split('-');
                    Rewards.Add(short.Parse(result[0]), ushort.Parse(result[1]));
                });
            }

            if (IsBought) AddStoreItemEventHandler?.Invoke(BuyID, true);
        }

        public void Dispose()
        {
            UI = null;
            Icon = null;
            Price = null;
            Name = null;
            BuyID = null;
            Description = null;
            AlternativePrice = null;
            AlternativeBuyID = null;
            AddStoreItemEventHandler = null;
        }
    }
}