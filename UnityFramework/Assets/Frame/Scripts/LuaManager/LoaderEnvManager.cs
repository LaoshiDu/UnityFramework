using System.IO;
using UnityEngine;
using XLua;

public class LoaderEnvManager
{
    private static LuaEnv env;
    private static LoaderEnvManager instance;
    public static LoaderEnvManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoaderEnvManager();
            }
            if (env == null)
            {
                env = new LuaEnv();
            }
            return instance;
        }
    }

    public void Dispose()
    {
        env.Dispose();
    }

    /// <summary>
    /// 执行lua代码
    /// </summary>
    /// <param name="path"></param>
    public void ExecutionLua(string path)
    {
        env.AddLoader(MyLoader);
        string str = "require(\"" + path + "\")";
        env.DoString(str);
    }

    private byte[] MyLoader(ref string path)
    {
        string realPath = Application.dataPath + "/Lua/" + path + ".lua";
        
        if (File.Exists(realPath))
        {
            return File.ReadAllBytes(realPath);
        }
        else
        {
            return null;
        }
    }

    public LuaTable Global
    {
        get
        {
            return env.Global;
        }
    }
}