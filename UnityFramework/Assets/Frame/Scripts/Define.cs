#region 枚举
public enum EventName
{
    /// <summary>
    /// loading条进度(加载进度)
    /// </summary>
    LoadingProgess
}

public enum PanelState
{
    /// <summary>
    /// 显示并且可以操作
    /// </summary>
    Showing = 0,

    /// <summary>
    /// 显示但不能操作
    /// </summary>
    Pause = 1,

    /// <summary>
    /// 隐藏
    /// </summary>
    Hide = 2
}
public enum PanelType
{
    /// <summary>
    /// 不被隐藏的面板
    /// </summary>
    Base = 0,

    /// <summary>
    /// 弹窗
    /// </summary>
    PopupWindow = 1,

    /// <summary>
    /// 可以同时存在并且可以同时操作的面板
    /// </summary>
    Panel = 2,

    /// <summary>
    /// 提示面板
    /// </summary>
    Tip = 3,

    /// <summary>
    /// Loading界面
    /// </summary>
    Loading = 4
}

public enum JoyStickType
{
    Static,
    Dynamic
}

public enum AssetLoadType
{
    Resources,
    AssetBundle
}

#endregion

#region 结构体
/// <summary>
/// 数字转换格式
/// </summary>
public struct NumFormat
{
    /// <summary>
    /// 数据临界点
    /// </summary>
    public double Value { get; set; }
    /// <summary>
    /// 标志、单位
    /// </summary>
    public string symbol { get; set; }
}
#endregion

#region 类
#endregion