using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//避免产生GC
//[GCOptimize]
public struct LuaBootstrap
{
    public delegate void LifeTime();
    public delegate void UpdateUI(params object[] args);

    public LifeTime Start;
    public LifeTime Update;
    public LifeTime OnDestroy;
    public UpdateUI UpdateShowUI;
}

public class LuaStart : MonoBehaviour
{
    private LuaBootstrap LB;
    void Start()
    {
        LoaderEnvManager.Instance.ExecutionLua("GameStart");
        //将Lua脚本映射过来
        LB = LoaderEnvManager.Instance.Global.Get<LuaBootstrap>("GameStart");
        //执行Lua脚本的Start方法
        LB.Start();
    }
}