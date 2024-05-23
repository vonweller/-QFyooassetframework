# QFyooassetframework

#### 介绍
完全不侵入QFramework源代码接入Yooassets资源管理框架

#### 软件架构
QFramework
Yooassets
Luban

#### 安装教程

1.  克隆项目Unity打开项目即可

2.  或者自行按照:

    1、新建立Unity项目

    2、接入Qframework

    3、接入Yooasset2.X及以上版本

    4、复制QFYooasset文件夹到unity项目中即可

    5、Luban接入Unity不需要额外的操作，因为已经将代码继承在了QFluan中

3.  注意

    1、QFYooasset中的QFYooAssetsAudioLoaderPool、QFYooAssetsPanelLoaderPool、QFYooAssetRes脚本写死了Yooassets加载的方式是简写加载(即可寻址包名加载,如果要使用全路径需要自行改写这3个脚本中的路径加载)

    2、加载方式非常简单：将QFYooassets_Init脚本挂载到场景初始化对象上作为程序入口即可。

    3、当前没有实现资源下载进度动画，只单纯的实现了资源加载debu进度，需要实现加载动画的自行调用QFYooassets_Init脚本中 OnDownloadProgressUpdateFunction中的返回值即可

    4、导表的使用注意直接使用LubanTools文件夹中的gen.bat即可自动生成对应json与c#代码到你当前的项目中！

    5、注意LubanTools文件夹位置应该位于你unity项目中且与你的Assets位于同目录下

    6、具体luban导表的自定义请查看Luban官方文档，简单易懂

#### 使用说明
    1、代码演示
IEnumerator StartGames()
        {
            // 添加创建器加载模板与初始化接入yoo资源管理
            ResKit.InitAsync().ToAction().Start(this);
            UIKit.Config.PanelLoaderPool = new QFYooAssetsPanelLoaderPool();
            AudioKit.Config.AudioLoaderPool = new QFYooAssetsAudioLoaderPool();
            ResFactory.AddResCreator<QFYooAssetResCreator>();
            
            //这里使用的是yoo的简写路径、可以再QFYooAssetsPanelLoaderPool中声明使用简写的路径
            UIKit.OpenPanelAsync<UIGameStart>().ToAction().Start(this);
            AudioKit.PlayMusic("game_pass",false);
            
            //开始使用
            var mresLoader = ResLoader.Allocate();
            mresLoader.LoadSync<GameObject>("yoo:test").Instantiate().Position(this.Position()).Show();
            //场景加载目前只能这样加载
            mresLoader.Add2Load("yoo:games", (a, b) =>
            {
                if (a)
                {
                    var oper = SceneManager.LoadSceneAsync("games",LoadSceneMode.Single);
                    while (true)
                    {
                        if (oper.progress>=0.9f)
                        {
                            break;
                        }
                        Debug.Log($"加载场景中...{oper.progress}");
                        Task.Delay(100);
                    }
                }
            });
                     
            
            mresLoader.LoadAsync();
            
            mresLoader.Recycle2Cache();
            mresLoader = null;
            UIKit.ClosePanel<UIGameStart>();
            //测试luban加载
            this.gameObject.AddComponent<QFluabanMain>();
            yield break;
        }
        
    2、QFLuban代码示例
     void Start()
    {
        var tablesCtor = typeof(cfg.Tables).GetConstructors()[0];
        var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];
        // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
        System.Delegate loader = loaderReturnType == typeof(ByteBuf) ?
            new System.Func<string, ByteBuf>(LoadByteBuf)
            : (System.Delegate)new System.Func<string, JSONNode>(LoadJson);
        var tables = (cfg.Tables)tablesCtor.Invoke(new object[] {loader});
        
        Debug.Log(tables.TbItem.Get(10001).Name);

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

    3、其余使用资源加载方式与reskit、AudioKit、UIKit完全相同自行查看QF文档手册即可

#### 参与贡献

1.  QFramework，地址：https://qframework.cn/qf
2.  Luabn,地址https://luban.doc.code-philosophy.com/docs/manual/loadconfigatruntime
2.  感谢QF群友大佬 “天下游”的全程协助



#### 特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  Gitee 官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解 Gitee 上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是 Gitee 最有价值开源项目，是综合评定出的优秀开源项目
5.  Gitee 官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  Gitee 封面人物是一档用来展示 Gitee 会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
