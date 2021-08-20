using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace WKC
{
    public class AtlasManager : BaseMgr<AtlasManager>
    {
        private static Dictionary<string, SpriteAtlas> spriteAtlases;
        public void Init()
        {
            spriteAtlases = new Dictionary<string, SpriteAtlas>();

            SpriteAtlas[] spriteAtlas = null;
            switch (GameConfig.Instance.loadType)
            {
                case AssetLoadType.Resources:
                    spriteAtlas = Resources.LoadAll<SpriteAtlas>("SpriteAtlases");
                    break;
                case AssetLoadType.AssetBundle:
                    spriteAtlas = AssetBundleManager.Instance.LoadAllRes<SpriteAtlas>("spriteatlases");
                    break;
            }

            for (int i = 0; i < spriteAtlas.Length; i++)
            {
                spriteAtlases.Add(spriteAtlas[i].name, spriteAtlas[i]);
            }
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="atlas"></param>
        /// <param name="name"></param>
        /// <param name="nativeSize">是否保持图片大小</param>
        public void SetSprite(Image image, string atlas, string name, bool nativeSize = false)
        {
            if (!spriteAtlases.ContainsKey(atlas)) return;
            Sprite sprite = spriteAtlases[atlas].GetSprite(name);
            image.sprite = sprite;
            if (nativeSize)
            {
                image.SetNativeSize();
                RectTransform rect = image.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
            }
        }
    }
}