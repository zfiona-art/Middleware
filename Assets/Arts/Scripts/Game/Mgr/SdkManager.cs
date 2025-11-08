using System;
using TTSDK;
using UnityEngine;

public class SdkManager : Singleton<SdkManager>
{
    private bool IsDouPlatform()
    {
        #if MINIGAME_SUBPLATFORM_DOUYIN
            return true;
        #endif
        return false;
    }
    public override void Init()
    {
        if (!IsDouPlatform()) return;
        TT.InitSDK();
    }
    
    public void Login(Action<bool,string,string> callback)
    {
        if (!IsDouPlatform()) callback.Invoke(false, null, null);
        TT.GetUserInfo(GetLoginOk(callback),GetLoginError(callback));
    }

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
    
    public void ShowAd(Action<bool,int> callback) //是否看完视频，看了几次
    {
        if (!IsDouPlatform()) callback.Invoke(true, 0);
        TT.CreateRewardedVideoAd("", callback);
    }

}
