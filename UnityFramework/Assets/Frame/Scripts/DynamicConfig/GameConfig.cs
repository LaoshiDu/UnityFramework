using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class GameConfig : Singleton<GameConfig>
    {
        [Header("版本号")]
        public string version = "";

        [Header("清除玩家通过Json保存的数据")]
        public bool isClearUserData;
    }
}