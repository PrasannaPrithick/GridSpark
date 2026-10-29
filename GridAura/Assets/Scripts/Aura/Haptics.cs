using System.Runtime.InteropServices;
using UnityEngine;

namespace Aura
{
    public static class Haptics
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void GridAuraHapticLight();
        [DllImport("__Internal")] static extern void GridAuraHapticMedium();
        [DllImport("__Internal")] static extern void GridAuraHapticHeavy();
        [DllImport("__Internal")] static extern void GridAuraHapticTick();
        [DllImport("__Internal")] static extern void GridAuraHapticMiss();
        [DllImport("__Internal")] static extern void GridAuraHapticHint();
        [DllImport("__Internal")] static extern void GridAuraHapticConfettiStart();
        [DllImport("__Internal")] static extern void GridAuraHapticStop();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        static AndroidJavaObject activity;
        static AndroidJavaObject vibrator;
        static AndroidJavaObject manager;
        static int[] motors;
        static int sdk;
        static bool androidReady;
#endif

        public static bool Enabled = true;

        public static void Place()
        {
            Native(1);
        }

        public static void Impact(bool heavy = false)
        {
            Native(heavy ? 3 : 2);
        }

        public static void Tick()
        {
            Native(0);
        }

        public static void Miss()
        {
            Native(4);
        }

        public static void Hint()
        {
            Native(5);
        }

        public static void Confetti()
        {
            Native(6);
        }

        public static void Stop()
        {
            Native(7);
        }

        static void Native(int kind)
        {
            if (!Enabled)
                return;
#if UNITY_EDITOR
            return;
#elif UNITY_IOS
            switch (kind)
            {
                case 0: GridAuraHapticTick(); break;
                case 1: GridAuraHapticMedium(); break;
                case 2: GridAuraHapticMedium(); break;
                case 3: GridAuraHapticHeavy(); break;
                case 4: GridAuraHapticMiss(); break;
                case 5: GridAuraHapticHint(); break;
                case 6: GridAuraHapticConfettiStart(); break;
                case 7: GridAuraHapticStop(); break;
            }
#elif UNITY_ANDROID
            AndroidKind(kind);
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        static void EnsureAndroid()
        {
            if (androidReady)
                return;
            try
            {
                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                    sdk = version.GetStatic<int>("SDK_INT");
                using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    activity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity == null)
                    return;
                if (sdk >= 31)
                {
                    manager = activity.Call<AndroidJavaObject>("getSystemService", "vibrator_manager");
                    if (manager != null)
                    {
                        motors = manager.Call<int[]>("getVibratorIds");
                        vibrator = manager.Call<AndroidJavaObject>("getDefaultVibrator");
                    }
                }
                if (vibrator == null)
                    vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                androidReady = vibrator != null;
            }
            catch
            {
                androidReady = false;
            }
        }

        static void AndroidKind(int kind)
        {
            EnsureAndroid();
            if (!androidReady)
                return;
            try
            {
                if (kind == 7)
                {
                    Cancel();
                    return;
                }
                if (kind == 6)
                {
                    OneShot(35, 220);
                    return;
                }
                if (kind == 5)
                {
                    Wave(new long[] { 0, 30, 70, 40, 50, 70 }, new int[] { 0, 200, 0, 255, 0, 140 }, -1);
                    return;
                }
                if (kind == 4)
                {
                    Wave(new long[] { 0, 40, 50, 80 }, new int[] { 0, 255, 0, 90 }, -1);
                    return;
                }
                if (kind == 0)
                    OneShot(12, 70);
                else if (kind == 3)
                    OneShot(40, 255);
                else
                    OneShot(22, 200);
            }
            catch
            {
            }
        }

        static void OneShot(long ms, int amplitude)
        {
            if (sdk >= 29)
            {
                int preset = amplitude >= 240 ? 5 : (amplitude <= 80 ? 2 : 0);
                using (var fx = new AndroidJavaClass("android.os.VibrationEffect"))
                using (var effect = fx.CallStatic<AndroidJavaObject>("createPredefined", preset))
                    Play(effect);
                return;
            }
            if (sdk >= 26)
            {
                using (var fx = new AndroidJavaClass("android.os.VibrationEffect"))
                using (var effect = fx.CallStatic<AndroidJavaObject>("createOneShot", ms, amplitude))
                    Play(effect);
                return;
            }
            vibrator.Call("vibrate", ms);
        }

        static void Wave(long[] times, int[] amps, int repeat)
        {
            if (sdk >= 26)
            {
                using (var fx = new AndroidJavaClass("android.os.VibrationEffect"))
                using (var effect = fx.CallStatic<AndroidJavaObject>("createWaveform", times, amps, repeat))
                    Play(effect);
                return;
            }
            vibrator.Call("vibrate", times, repeat);
        }

        static void Play(AndroidJavaObject effect)
        {
            if (manager != null && motors != null && motors.Length > 1)
            {
                for (int i = 0; i < motors.Length; i++)
                {
                    using (var motor = manager.Call<AndroidJavaObject>("getVibrator", motors[i]))
                        motor.Call("vibrate", effect);
                }
                return;
            }
            vibrator.Call("vibrate", effect);
        }

        static void Cancel()
        {
            if (manager != null && motors != null)
            {
                for (int i = 0; i < motors.Length; i++)
                {
                    using (var motor = manager.Call<AndroidJavaObject>("getVibrator", motors[i]))
                        motor.Call("cancel");
                }
            }
            if (vibrator != null)
                vibrator.Call("cancel");
        }
#endif
    }
}
