using System.Collections;
using UnityEngine;

namespace WKC
{
    public class GameStart : MonoBehaviour
    {
        void Start()
        {
            UIManager.Instance.Init();
            DataStorage.Instance.LoadJsonsData();
            LoadSceneManager.Instance.LoadSceneAsync("Level1", (args) =>
            {
                StartCoroutine(SaveDataToLocal());
            });

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