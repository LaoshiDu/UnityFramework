using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class GameConfig : Singleton<GameConfig>
    {
        [Header("�汾��")]
        public string version = "";

        [Header("������ͨ��Json���������")]
        public bool isClearUserData;
    }
}