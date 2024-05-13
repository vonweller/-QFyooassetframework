#define ENABLE_YOOASSET
#if ENABLE_YOOASSET
using QFramework;
using YooAsset;
using Object = UnityEngine.Object;


public class QFYooAssetRes : Res
{
    private AssetHandle _syncOperationHandle;
    private AssetHandle _asyncOperationHandle;
    private string _location;
    public override bool LoadSync()
    {
         _syncOperationHandle = YooAssets.LoadAssetSync<Object>(_location);
         mAsset = _syncOperationHandle.AssetObject;
         State = ResState.Ready;
         return true;
    }

    public override async void LoadAsync()
    {
        _asyncOperationHandle =YooAssets.LoadAssetSync<Object>(_location);
        await _asyncOperationHandle.Task;
        mAsset = _asyncOperationHandle.AssetObject;
        State = ResState.Ready;
    }

    protected override void OnReleaseRes()
    {
        if(_syncOperationHandle!=null) _syncOperationHandle.Release();
        if(_asyncOperationHandle!=null) _asyncOperationHandle.Release();
        mAsset = null;
        State = ResState.Waiting;
    }

    public static QFYooAssetRes Allocate(string name,string originalAssetName)
    {
        var res = SafeObjectPool<QFYooAssetRes>.Instance.Allocate();
        if (res != null)
        {
            if (originalAssetName.StartsWith("yoo://"))
            {
                originalAssetName = originalAssetName.Substring("yoo://".Length);
            }
            if(originalAssetName.StartsWith("yoo:"))
            {
                originalAssetName = originalAssetName.Substring("yoo:".Length);
            }
            res.AssetName = name;
            res._location = originalAssetName;
        }
        return res;
    }
}

public class QFYooAssetResCreator : IResCreator
{
    public IRes Create(ResSearchKeys resSearchKeys)
    {
        QFYooAssetRes res = QFYooAssetRes.Allocate(resSearchKeys.AssetName,resSearchKeys.OriginalAssetName);
        res.AssetType = resSearchKeys.AssetType;
        return res;
    }

    public bool Match(ResSearchKeys resSearchKeys)
    {
        return resSearchKeys.OriginalAssetName.StartsWith("yoo://") || resSearchKeys.OriginalAssetName.StartsWith("yoo:");
        
    }
}
#endif
