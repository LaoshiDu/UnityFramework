namespace WKC
{
    public class UserData
    {
        /// <summary>
        /// °æ±¾ºÅ
        /// </summary>
        public string version;

        public int gold;

        public UserData()
        {
            version = GameConfig.Instance.version;
        }
    }
}