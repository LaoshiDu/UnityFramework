using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class GameConfig : Singleton<GameConfig>
    {
        [HideInInspector]
        public string version = "0";

        [Header("������ͨ��Json���������")]
        public bool isClearUserData;

        [Header("��Դ���ط�ʽ")]
        public AssetLoadType loadType = AssetLoadType.Resources;
    }
}