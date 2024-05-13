using System;
using System.Collections;
using System.IO;
using QFramework;
using QFramework.Example;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using YooAsset;


    public class QFYooassets_Init:MonoBehaviour
    {
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        [SerializeField]
        private string defaultHostServer="http://127.0.0.1/CDN/Android/v1.0";
        [SerializeField]
        private string fallbackHostServer="http://127.0.0.1/CDN/Android/v1.0";
        
        /*//如果加入热更新
        [SerializeField]
        private string HotDllName = "hotfix.dll";
        [SerializeField]
        private string HotPrefabName = "HotFix_Import";*/
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        #region Yooasset初始化与资源下载验证
        private IEnumerator Start()
        {
            // 初始化资源系统
            YooAssets.Initialize();
            // 创建默认的资源包
            var package = YooAssets.CreatePackage("DefaultPackage");
            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);
            
            switch (PlayMode)
            {
                case EPlayMode.EditorSimulateMode:
                    var initParametersEditorSimulateMode = new EditorSimulateModeParameters();
                    var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "DefaultPackage");
                    initParametersEditorSimulateMode.SimulateManifestFilePath  = simulateManifestFilePath;
                    yield return package.InitializeAsync(initParametersEditorSimulateMode);
                    break;
                case EPlayMode.OfflinePlayMode:
                    var initParametersOfflinePlayMode = new OfflinePlayModeParameters();
                    yield return package.InitializeAsync(initParametersOfflinePlayMode);
                    break;
                case EPlayMode.HostPlayMode:
                    // 注意：GameQueryServices.cs 太空战机的脚本类，详细见StreamingAssetsHelper.cs
                    var initParametersHostPlayMode = new HostPlayModeParameters();
                    initParametersHostPlayMode.BuildinQueryServices = new GameQueryServices(); 
                    initParametersHostPlayMode.DecryptionServices = new FileOffsetDecryption();
                    initParametersHostPlayMode.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    var initOperationHostPlayMode = package.InitializeAsync(initParametersHostPlayMode);
                    yield return initOperationHostPlayMode;
                    if(initOperationHostPlayMode.Status == EOperationStatus.Succeed)
                    {
                        Debug.Log("资源包初始化成功！");
                    }
                    else 
                    {
                        Debug.LogError($"资源包初始化失败：{initOperationHostPlayMode.Error}");
                    }
                    break;
                case EPlayMode.WebPlayMode:
                    // 注意：GameQueryServices.cs 太空战机的脚本类，详细见StreamingAssetsHelper.cs
                    var initParameters = new WebPlayModeParameters();
                    initParameters.BuildinQueryServices = new GameQueryServices();
                    initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    var initOperation = package.InitializeAsync(initParameters);
                    yield return initOperation;
    
                    if(initOperation.Status == EOperationStatus.Succeed)
                    {
                        Debug.Log("资源包初始化成功！");
                    }
                    else 
                    {
                        Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                    }
                    yield break;
            }
            
            var operation = package.UpdatePackageVersionAsync();
            yield return operation;
            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                string packageVersion = operation.PackageVersion;
                Debug.Log($"Updated package Version : {packageVersion}");
                var operation2 = package.UpdatePackageManifestAsync(packageVersion);
                yield return operation2;
                if (operation.Status == EOperationStatus.Succeed)
                {
                    //更新成功
                    yield  return StartCoroutine(Download());
                 
                }
                else
                {
                    //更新失败
                    Debug.LogError(operation.Error);
                }
            }
            else
            {
                //更新失败
                Debug.LogError(operation.Error);
            }
            
        }
        
        IEnumerator Download()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var package = YooAssets.GetPackage("DefaultPackage");
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
    
            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {        
                //直接进入游戏
                Debug.Log("没有需要下载的资源进入游戏成功");
               // yield return  StartCoroutine(StartGames());
            }
            //需要下载的文件总数和总大小
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;    
            Debug.Log($"下载资源总数:{totalDownloadCount} 大小:{totalDownloadBytes}");
            //注册回调方法
            downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
            downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
            downloader.OnDownloadOverCallback = OnDownloadOverFunction;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

            //开启下载
            downloader.BeginDownload();
            yield return downloader;

            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
            {
                //下载成功
                Debug.Log("下载成功进入游戏成功");
                yield return  StartCoroutine(StartGames());
            }
            else
            {
                //下载失败
                Debug.LogError("下载资源失败");
            }
        }
        #endregion
        
        #region yooasset下载回调函数
        /// <summary>
        /// 下载数据大小
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sizeBytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
        {
            Debug.Log(string.Format("开始下载：文件名：{0}, 文件大小：{1}", fileName, sizeBytes));
        }
        /// <summary>
        /// 下载完成与否
        /// </summary>
        /// <param name="isSucceed"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDownloadOverFunction(bool isSucceed)
        {
            Debug.Log("下载" + (isSucceed ? "成功" : "失败"));
        }

        /// <summary>
        /// 更新中
        /// </summary>
        /// <param name="totalDownloadCount"></param>
        /// <param name="currentDownloadCount"></param>
        /// <param name="totalDownloadBytes"></param>
        /// <param name="currentDownloadBytes"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            Debug.Log(string.Format("文件总数：{0}, 已下载文件数：{1}, 下载总大小：{2}, 已下载大小：{3}", totalDownloadCount, currentDownloadCount, totalDownloadBytes, currentDownloadBytes));
        
        }
        /// <summary>
        /// 下载出错
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="error"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnDownloadErrorFunction(string fileName, string error)
        {
            Debug.LogError(string.Format("下载出错：文件名：{0}, 错误信息：{1}", fileName, error));
        }
        #endregion
        
        #region yooasset远端资源地址查询服务类
        public class GameQueryServices : IBuildinQueryServices
        {
            /// <summary>
            /// 查询内置文件的时候，是否比对文件哈希值
            /// </summary>
            public static bool CompareFileCRC = false;

            public bool Query(string packageName, string fileName, string fileCRC)
            {
                // 注意：fileName包含文件格式
                return StreamingAssetsHelper.FileExists(packageName, fileName, fileCRC);
            }
        }
        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }
            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }

        /// <summary>
        /// 资源文件偏移加载解密类
        /// </summary>
        private class FileOffsetDecryption : IDecryptionServices
        {
            /// <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
            }

            private static ulong GetFileOffset()
            {
                return 32;
            }
        }
        
        #endregion
        
        IEnumerator StartGames()
        {
            Debug.Log("进入游戏成功");

            // 添加创建器加载模板与初始化接入yoo资源管理
            ResKit.InitAsync().ToAction().Start(this);
            UIKit.Config.PanelLoaderPool = new QFYooAssetsPanelLoaderPool();
            AudioKit.Config.AudioLoaderPool = new QFYooAssetsAudioLoaderPool();
            ResFactory.AddResCreator<QFYooAssetResCreator>();
            
            //这里使用的是yoo的简写路径、可以再QFYooAssetsPanelLoaderPool中声明使用简写的路径
            UIKit.OpenPanelAsync<UIGameStart>().ToAction().Start(this);
            AudioKit.PlayMusic("game_pass",false);
            
            
            /*
            //如果有热更新程序集
            var package= YooAssets.GetPackage("DefaultPackage");
            var handlefiles = package.LoadRawFileAsync(HotDllName);
            yield return handlefiles;
            System.Reflection.Assembly.Load(handlefiles.GetRawFileData());
            */
            
            //开始使用
            var mresLoader = ResLoader.Allocate();
            mresLoader.LoadSync<GameObject>("yoo:test").Instantiate().Position(this.Position()).Show();
            
            //场景加载目前只能这样加载
            mresLoader.Add2Load("yoo:games", (a, b) =>
            {
                if (a)
                {
                    var oper = SceneManager.LoadSceneAsync("games",LoadSceneMode.Single);
                }
            });
            
            mresLoader.LoadAsync();
            
            mresLoader.Recycle2Cache();
            mresLoader = null;
            UIKit.ClosePanel<UIGameStart>();
        yield break ;
        }
        
    }
