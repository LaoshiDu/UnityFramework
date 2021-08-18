using System;
using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    [Serializable]
    public class UserData
    {
        [Header("°æ±¾ºÅ")]
        /// <summary>
        /// °æ±¾ºÅ
        /// </summary>
        public string version;

        public int gold;

        public Dictionary<int, string> test = new Dictionary<int, string>();

        [SerializeField]
        private List<Dic> testDic = new List<Dic>();

        public UserData()
        {
            for (int i = 0; i < testDic.Count; i++)
            {
                test.Add(testDic[i].key,testDic[i].value);
            }
            ModifyData();
        }

        public void ModifyData()
        {
            test.Clear();
            for (int i = 0; i < testDic.Count; i++)
            {
                test.Add(testDic[i].key, testDic[i].value);
            }
        }
    }

    [Serializable]
    public class Dic
    {
        public int key;
        public string value;
    }
}