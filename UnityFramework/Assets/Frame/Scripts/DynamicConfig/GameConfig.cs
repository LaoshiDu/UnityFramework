using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class GameConfig : Singleton<GameConfig>
    {
        [HideInInspector]
        public StartIEnumerator startie;

        [Header("�汾��")]
        public string version = "";

        [Header("������ͨ��Json���������")]
        public bool isClearUserData;
    }
}