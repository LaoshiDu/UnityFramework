using UnityEngine;

namespace WKC
{
    public class GameStart : MonoBehaviour
    {
        private void Awake()
        {
            FrameManager.Instance.Init(StartGame);
        }
        
        private void StartGame(params object[] args)
        {
            DataStorage.Instance.LoadJsonsData();
            LoadSceneManager.Instance.LoadSceneAsync("Level1", (e) =>
            {
                TimeManager.Instance.UpdatePerSecondEventHandler += SaveDataToLocal;
                UIManager.Instance.ShowPanel(PanelName.MainMenuPanel);
            });
        }
        
        private void SaveDataToLocal()
        {
            DataStorage.Instance.userdata.gold++;
            DataStorage.Instance.SaveJsonDataToLocal();
        }
    }
}