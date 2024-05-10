# QFyooassetframework

#### 介绍
完全不侵入QFramework源代码接入Yooassets资源管理框架

#### 软件架构
QFramework
YooassetS


#### 安装教程

1.  克隆项目Unity打开项目即可

2.  或者自行按照:
    1、新建立Unity项目
    2、接入Qframework
    3、接入Yooasset2.X及以上版本
    4、复制QFYooasset文件夹到unity项目中即可

3.  注意
    1、QFYooasset中的QFYooAssetsAudioLoaderPool、QFYooAssetsPanelLoaderPool、QFYooAssetRes脚本写死了Yooassets加载的方式是简写加载(即可寻址包名加载,如果要使用全路径需要自行改写这3个脚本中的路径加载)
    2、加载方式非常简单：将QFYooassets_Init脚本挂载到场景初始化对象上作为程序入口即可。
    3、当前没有实现资源下载进度动画，只单纯的实现了资源加载debu进度，需要实现加载动画的自行调用QFYooassets_Init脚本中 OnDownloadProgressUpdateFunction中的返回值即可

#### 使用说明
    1、代码演示
    IEnumerator StartGames()
            {
                Debug.Log("进入游戏成功");

                // 添加创建器加载模板与初始化接入yoo资源管理
                ResKit.InitAsync().ToAction().Start(this);//初始化全局调用一次即可
                UIKit.Config.PanelLoaderPool = new QFYooAssetsPanelLoaderPool();//初始化全局调用一次即可
                AudioKit.Config.AudioLoaderPool = new QFYooAssetsAudioLoaderPool();//初始化全局调用一次即可
                ResFactory.AddResCreator<QFYooAssetResCreator>();//初始化全局调用一次即可


                //这里使用的是yoo的简写路径、可以再QFYooAssetsPanelLoaderPool中声明使用简写的路径
                //演示加载UI
                UIKit.OpenPanelAsync<UIGameStart>().ToAction().Start(this);
                //演示加载声音
                AudioKit.PlayMusic("game_pass",false);
                
                        
                //开始使用
                var mresLoader = ResLoader.Allocate();
                //演示加载跑通对象
                mresLoader.LoadSync<GameObject>("yoo:test").Instantiate().Position(this.Position()).Show();
                //mresLoader.LoadSceneAsync("yoo:games",onStartLoading: _ => { });
                mresLoader.Recycle2Cache();
            yield break ;
            }
        
    2、其余使用资源加载方式与reskit、AudioKit、UIKit完全相同自行查看QF文档手册即可

#### 参与贡献

1.  感谢QFramework，地址：https://qframework.cn/qf
2.  感谢QF群友大佬 “天下游”的全程协助



#### 特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  Gitee 官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解 Gitee 上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是 Gitee 最有价值开源项目，是综合评定出的优秀开源项目
5.  Gitee 官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  Gitee 封面人物是一档用来展示 Gitee 会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
