using UnityEngine;

namespace AnnouncementCore
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private void OnEnable()
        {
            try
            {
                Debug.Log("AnnouncementCore Mod已启用");

                AnnouncementCore.Core.AnnouncementManager.WakeUp();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"AnnouncementCore Mod启用异常: {e}");
            }
        }

        private void OnDisable()
        {
            try
            {
                Debug.Log("AnnouncementCore Mod被禁用");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"AnnouncementCore Mod禁用异常: {e}");
            }
        }
    }
}
