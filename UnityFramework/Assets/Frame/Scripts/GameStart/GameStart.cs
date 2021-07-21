using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class GameStart : MonoBehaviour
    {
        void Start()
        {
            DataStorage.Instance.LoadJsonsData();
            UIManager.Instance.Init();
            StartCoroutine(SaveDataToLocal());
        }

        IEnumerator SaveDataToLocal()
        {
            while (true)
            {
                DataStorage.Instance.userdata.gold++;
                DataStorage.Instance.SaveJsonDataToLocal();
                yield return new WaitForSeconds(1f);
            }
        }
    }
}