namespace WKC
{
    public class UserData
    {
        /// <summary>
        /// �汾��
        /// </summary>
        public string version;

        public int gold;

        public UserData()
        {
            version = GameConfig.Instance.version;
        }
    }
}