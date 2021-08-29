using System;
using UnityEngine.UI;
using WKC;

public class LoadingPanel : BasePanel
{
    private Image progressImg;
    private Text progressText;

    public override void Init()
    {
        base.Init();
        progressImg = GetUIComponent<Image>("ProgressBg/Progress");
        progressText = GetUIComponent<Text>("Progress");
    }

    public override void Show()
    {
        base.Show();
        EventCenterManager.Instance.AddEventListener(EventName.LoadingProgess, UpdateProgress);
        AtlasManager.Instance.SetSprite(progressImg, "TestAtlas", "Test");
    }

    private void UpdateProgress(object[] args)
    {
        progressImg.fillAmount = (float)args[0];
        progressText.text = Math.Round((float)args[0], 2) * 100 + "%";
    }

    public override void Hide()
    {
        base.Hide();
        EventCenterManager.Instance.RemoveEventListener(EventName.LoadingProgess, UpdateProgress);
    }
}