namespace WKC
{
    public class FrameManager : BaseMgr<FrameManager>
    {
        /// <summary>
        /// ¿ò¼ÜÆô¶¯
        /// </summary>
        /// <param name="call"></param>
        public void Init(EventCall call)
        {
            callback = call;
            AssetBundleCheckManager.Instance.Init((args) =>
            {
                ConfigManager.Instance.Init(FrameInit);
            });
        }

        private void FrameInit(params object[] args)
        {
            AtlasManager.Instance.Init();
            UIManager.Instance.Init();
            ObjectPoolManager.Instance.Init();
            callback?.Invoke();
        }
    }
}