using System;
using UnityEngine;

namespace QFramework
{
    public class QFYooAssetsAudioLoaderPool:AbstractAudioLoaderPool
    {
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override IAudioLoader CreateLoader()
        {
            return new QFAssetsAudioLoader();
        }
        
        class QFAssetsAudioLoader : IAudioLoader
        {
            private AudioClip mClip;
        
            public AudioClip Clip => mClip;

            private ResLoader mResLoader;
        
            public AudioClip LoadClip(AudioSearchKeys audioSearchKeys)
            {
                if (mResLoader == null)
                {
                    mResLoader = ResLoader.Allocate();
                }
                return mResLoader.LoadSync<AudioClip>($"yoo:{audioSearchKeys.AssetName}");
            }

            public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
            {
                if (mResLoader == null)
                {
                    mResLoader = ResLoader.Allocate();
                }

                mResLoader.Add2Load<AudioClip>($"yoo:{audioSearchKeys.AssetName}", (b, res) =>
                {
                    mClip = res.Asset as AudioClip;
                    onLoad(b, res.Asset as AudioClip);
                });

                mResLoader.LoadAsync();
            }

            public void Unload()
            {
                mClip = null;
                mResLoader?.Recycle2Cache();
                mResLoader = null;
            }
        
        }
        
    }
    

}