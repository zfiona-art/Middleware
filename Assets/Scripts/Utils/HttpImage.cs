using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class HttpImage
{
    private const string _CACHE = "ImageCache";
    private static string cachePath = Application.persistentDataPath + "/" + _CACHE + "/";
    private static Dictionary<string, Sprite> sprDic = new Dictionary<string, Sprite>();
    private static int timeOut = 5;

    static HttpImage()
    {
        if (!Directory.Exists(cachePath))
        {
            Directory.CreateDirectory(cachePath);
        }
    }

    public static void ClearCache()
    {
        sprDic.Clear();
        if (Directory.Exists(cachePath))
        {
            Directory.Delete(cachePath, true);
        }
        Directory.CreateDirectory(cachePath);
    }

    public static void AsyncLoad(string url, Action<Sprite> action)
    {
        Debug.Assert(!string.IsNullOrEmpty(url), "Image url can't be null");
        if (sprDic.ContainsKey(url))
        {
            action?.Invoke(sprDic[url]);
            return;
        }
        string savePath = cachePath + ToolUtil.GetMD5FromString(url);
        if (File.Exists(savePath))
        {
            url = "file://" + cachePath + ToolUtil.GetMD5FromString(url);
            GameManager.Instance.StartRequestDirect(IDownload(url, null, action));
        }
        else
        {
            GameManager.Instance.StartRequestDirect(IDownload(url, savePath, action));
        }
    }

    public static void AsyncLoadWithoutCache(string url, Action<Sprite> action)
    {
        Debug.Assert(!string.IsNullOrEmpty(url), "Image url can't be null");

        GameManager.Instance.StartRequestDirect(AsyncLoadWithoutCacheInner(url, action));

    }


    static IEnumerator IDownload(string url, string savePath, Action<Sprite> action)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            uwr.timeout = timeOut;
            yield return uwr.SendWebRequest();
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                action?.Invoke(null);
            }
            else
            {
                if (!string.IsNullOrEmpty(savePath))
                    File.WriteAllBytes(savePath, uwr.downloadHandler.data);
                Texture2D tex2d = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.one * 0.5f);
                sprDic[url] = sprite;
                action?.Invoke(sprite);
            }
        }
    }


    private static IEnumerator AsyncLoadWithoutCacheInner(string url, Action<Sprite> action)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            uwr.timeout = timeOut;
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                action?.Invoke(null);
            }
            else
            {
                Texture2D tex2d = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.one * 0.5f);
                action?.Invoke(sprite);
            }
        }
    }
}
