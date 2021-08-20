using UnityEngine;
using UnityEngine.UI;
using WKC;

public class LoadingPanel : BasePanel
{
    private Image progress;

    public override void Init()
    {
        base.Init();
        progress = GetUIComponent<Image>("ProgressBg/Progress");
    }

    public override void Show()
    {
        base.Show();
        EventCenterManager.Instance.AddEventListener(EventName.LoadingProgess, UpdateProgress);
        AtlasManager.Instance.SetSprite(progress, "TestAtlas", "Test");
    }

    private void UpdateProgress(object[] args)
    {
        progress.fillAmount = (float)args[0];
    }

    public override void Hide()
    {
        base.Hide();
        EventCenterManager.Instance.RemoveEventListener(EventName.LoadingProgess, UpdateProgress);
    }
}