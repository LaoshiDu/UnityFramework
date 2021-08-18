using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class GameConfig : Singleton<GameConfig>
    {
        [HideInInspector]
        public string version = "0";

        [Header("清除玩家通过Json保存的数据")]
        public bool isClearUserData;

        [Header("资源加载方式")]
        public AssetLoadType loadType = AssetLoadType.Resources;
    }
}