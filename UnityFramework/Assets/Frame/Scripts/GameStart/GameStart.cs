using System.Collections;
using UnityEngine;

namespace WKC
{
    public class GameStart : MonoBehaviour
    {
        void Start()
        {
            DataStorage.Instance.LoadJsonsData();
            UIManager.Instance.Init();
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