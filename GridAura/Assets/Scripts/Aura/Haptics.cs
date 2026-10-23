using System.Runtime.InteropServices;
using UnityEngine;

namespace Aura
{
    public static class Haptics
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void GridAuraHapticLight();

        [DllImport("__Internal")]
        static extern void GridAuraHapticMedium();

        [DllImport("__Internal")]
        static extern void GridAuraHapticHeavy();
#endif

        public static void Place()
        {
            Pulse(0);
        }

        public static void Impact(bool heavy = false)
        {
            Pulse(heavy ? 2 : 1);
        }

        static void Pulse(int strength)
        {
            if (!Application.isMobilePlatform)
                return;
#if UNITY_IOS && !UNITY_EDITOR
            if (strength >= 2)
                GridAuraHapticHeavy();
            else if (strength == 1)
                GridAuraHapticMedium();
            else
                GridAuraHapticLight();
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidTap(strength >= 1 ? 0 : 1);
#endif
        }

        static void AndroidTap(int kind)
        {
            try
            {
                using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        try
                        {
                            using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                            using (var view = window.Call<AndroidJavaObject>("getDecorView"))
                                view.Call<bool>("performHapticFeedback", kind);
                        }
                        catch
                        {
                        }
                    }));
                }
            }
            catch
            {
            }
        }
    }
}
