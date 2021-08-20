namespace WKC
{
    public class GameStart : Singleton<GameStart>
    {
        private void Awake()
        {
            ConfigManager.Instance.Init(FrameInit);
        }

        private void FrameInit(params object[] args)
        {
            AtlasManager.Instance.Init();
            UIManager.Instance.Init();
            ObjectPoolManager.Instance.Init();
            StartGame();
        }

        private void StartGame()
        {
            DataStorage.Instance.LoadJsonsData();
            LoadSceneManager.Instance.LoadSceneAsync("Level1", (args) =>
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