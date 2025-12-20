using Duckov;
using UnityEngine;

namespace AnnouncementCore.Utility
{
    public static class AudioUtility
    {
        public static void PlayHoverSound()
        {
            try
            {
                string[] possiblePaths = {
                    "UI/hover",
                    "UI/Button/Hover",
                    "SFX/UI/Hover",
                    "UI/Menu/Hover"
                };

                foreach (var path in possiblePaths)
                {
                    try
                    {
                        var result = AudioManager.Post(path);
                        if (result != null)
                        {
                            //Debug.Log($"播放悬停音效成功: {path}");
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"播放悬停音效失败: {e.Message}");
            }
        }

        /// UI点击音效
        public static void PlayClickSound()
        {
            try
            {
                string[] possiblePaths = {
                    "UI/click",
                    "UI/Button/Click",
                    "SFX/UI/Click",
                    "UI/Menu/Click",
                    "UI/Button/Press"
                };

                foreach (var path in possiblePaths)
                {
                    try
                    {
                        var result = AudioManager.Post(path);
                        if (result != null)
                        {
                            //Debug.Log($"播放点击音效成功: {path}");
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"播放点击音效失败: {e.Message}");
            }
        }
    }
}
