using System;
using UnityEngine;
using cfg;
using Luban;
using QFramework;
using SimpleJSON;

public class QFluabanMain : MonoSingleton<QFluabanMain>
{

    public Tables tables;

    public void Init()
    {
        var tablesCtor = typeof(cfg.Tables).GetConstructors()[0];
        var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];
        // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
        System.Delegate loader = loaderReturnType == typeof(ByteBuf) ?
            new System.Func<string, ByteBuf>(LoadByteBuf)
            : (System.Delegate)new System.Func<string, JSONNode>(LoadJson);      
        tables = (cfg.Tables)tablesCtor.Invoke(new object[] {loader});
    }

    /// <summary>
    /// 加载二进制格式的配置文件
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    private ByteBuf LoadByteBuf(string files)
    {
        var _mloader = ResLoader.Allocate();
        var jsons=_mloader.LoadSync<TextAsset>($"yoo:{files}").bytes;
        _mloader.Recycle2Cache();
        _mloader = null;
        return new ByteBuf(jsons);
    }

    /// <summary>
    /// 加载json格式的配置文件
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    private JSONNode LoadJson(string files)
    {
        var _mloader = ResLoader.Allocate();
        var jsons=_mloader.LoadSync<TextAsset>($"yoo:{files}").text;
        _mloader.Recycle2Cache();
        _mloader = null;
        return JSON.Parse(jsons);
        
    }
}
