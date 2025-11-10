using System;
using UnityEngine;
#if MINIGAME_SUBPLATFORM_DOUYIN
using TTSDK;
#endif


public class SdkManager : Singleton<SdkManager>
{
    public override void Init()
    {
#if MINIGAME_SUBPLATFORM_DOUYIN
         TT.InitSDK();
#endif
        
    }
    
    public void Login(Action<bool,string,string> callback)
    {
#if MINIGAME_SUBPLATFORM_DOUYIN
        TT.GetUserInfo(GetLoginOk(callback),GetLoginError(callback));
#else
        callback.Invoke(false, null, null);
#endif
        
    }

#if MINIGAME_SUBPLATFORM_DOUYIN
    private TTAccount.OnGetUserInfoSuccessCallback GetLoginOk(Action<bool,string,string> callback)
    {
        return (ref TTUserInfo info) =>
        {
            callback.Invoke(true,info.avatarUrl,info.nickName);
        };
    }
    private TTAccount.OnGetUserInfoFailedCallback GetLoginError(Action<bool,string,string> callback)
    {
        return errMsg =>
        {
            callback.Invoke(false, null, null);
            Debug.LogError(errMsg);
        };
    }
#endif
    
    public void ShowAd(Action<bool,int> callback) //是否看完视频，看了几次
    {
#if MINIGAME_SUBPLATFORM_DOUYIN
        TT.CreateRewardedVideoAd("", callback);
#else
        callback.Invoke(true, 0);
#endif
    }

}
