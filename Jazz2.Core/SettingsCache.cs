﻿namespace Jazz2
{
    public class SettingsCache
    {
        public enum ResizeMode
        {
            None,
            HQ2x,
            xBRZ3,
            xBRZ4,
            CRT,
            GB
        }

#if __ANDROID__
        public static ResizeMode Resize = ResizeMode.None;
#else
        public static ResizeMode Resize = ResizeMode.xBRZ3;
#endif
        public static float MusicVolume = 0.7f;
        public static float SfxVolume = 0.85f;
    }
}