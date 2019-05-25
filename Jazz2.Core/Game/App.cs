﻿using System;
using System.IO;
using System.Text.Json;
using Duality;
using Duality.Backend;
using Duality.IO;
using Duality.Resources;
using Jazz2.Game.Structs;
using Jazz2.Game.UI;
using Jazz2.Game.UI.Menu;
using Jazz2.Storage;

namespace Jazz2.Game
{
    public partial class App
    {
        private INativeWindow window;
        private bool isInstallationComplete;

        public string Title
        {
            get { return window.Title; }
            set { window.Title = App.AssemblyTitle + (string.IsNullOrEmpty(value) ? "" : " - " + value); }
        }

        public RefreshMode RefreshMode
        {
            get
            {
                return window.RefreshMode;
            }
            set
            {
                window.RefreshMode = value;
            }
        }

        public ScreenMode ScreenMode
        {
            get
            {
                return (window.ScreenMode & ~ScreenMode.Immersive);
            }
            set
            {
                ScreenMode previousValue = (window.ScreenMode & ~ScreenMode.Immersive);
                ScreenMode newValue = (value & ~ScreenMode.Immersive);
                if (previousValue == newValue) {
                    return;
                }

                window.ScreenMode = newValue | (window.ScreenMode & ScreenMode.Immersive);

                if ((value & (ScreenMode.FullWindow | ScreenMode.ChangeResolution)) == 0) {
                    window.Size = LevelRenderSetup.TargetSize;
                }
            }
        }

        public bool Immersive
        {
            get
            {
                return (window.ScreenMode & ScreenMode.Immersive) != 0;
            }
            set
            {
                if (value) {
                    window.ScreenMode |= ScreenMode.Immersive;
                } else {
                    window.ScreenMode &= ~ScreenMode.Immersive;
                }
            }
        }

        public App(INativeWindow window)
        {
            this.window = window;

            this.isInstallationComplete = IsInstallationComplete();

            // Load settings to cache
            SettingsCache.Resize = (SettingsCache.ResizeMode)Preferences.Get<byte>("Resize", (byte)SettingsCache.Resize);
            SettingsCache.MusicVolume = Preferences.Get<byte>("MusicVolume", (byte)(SettingsCache.MusicVolume * 100)) * 0.01f;
            SettingsCache.SfxVolume = Preferences.Get<byte>("SfxVolume", (byte)(SettingsCache.SfxVolume * 100)) * 0.01f;
        }

        public void ShowMainMenu(bool afterIntro)
        {
#if MULTIPLAYER
            if (net != null) {
                net.OnDisconnected -= OnNetworkDisconnected;
                net.Close();
                net = null;
            }
#endif

            if (!this.isInstallationComplete) {
                // If it's incomplete, check it again to be sure
                this.isInstallationComplete = IsInstallationComplete();
            }

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;

            ContentResolver.Current.ResetReferenceFlag();

            Scene.Current.DisposeLater();
            Scene.SwitchTo(new MainMenu(this, this.isInstallationComplete, afterIntro));

            ContentResolver.Current.ReleaseUnreferencedResources();
        }

        public void ChangeLevel(LevelInitialization carryOver = default(LevelInitialization))
        {
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;

            ContentResolver.Current.ResetReferenceFlag();

            if (string.IsNullOrEmpty(carryOver.LevelName)) {
                // Next level not specified, so show main menu
                ShowMainMenu(false);
            } else if (carryOver.LevelName == ":end") {
                // End of episode

                if (!string.IsNullOrEmpty(carryOver.LastEpisodeName) && carryOver.LastEpisodeName != "unknown") {
                    // ToDo: Implement time
                    Preferences.Set("EpisodeEnd_Time_" + carryOver.LastEpisodeName, (int)1);

                    if (carryOver.PlayerCarryOvers.Length == 1) {
                        // Save CarryOvers only in SinglePlayer mode
                        ref PlayerCarryOver player = ref carryOver.PlayerCarryOvers[0];

                        Preferences.Set("EpisodeEnd_Lives_" + carryOver.LastEpisodeName, (byte)player.Lives);
                        if (player.Ammo != null) {
                            Preferences.Set("EpisodeEnd_Ammo_" + carryOver.LastEpisodeName, player.Ammo);
                        }

                        // Save WeaponUpgrades only if at least one of them is upgraded
                        if (player.WeaponUpgrades != null) {
                            for (int i = 0; i < player.WeaponUpgrades.Length; i++) {
                                if (player.WeaponUpgrades[i] != 0) {
                                    Preferences.Set("EpisodeEnd_Upgrades_" + carryOver.LastEpisodeName, player.WeaponUpgrades);
                                    break;
                                }
                            }
                        }
                    }

                    // Remove existing continue data
                    Preferences.Remove("EpisodeContinue_Misc_" + carryOver.LastEpisodeName);
                    Preferences.Remove("EpisodeContinue_Level_" + carryOver.LastEpisodeName);
                    Preferences.Remove("EpisodeContinue_Ammo_" + carryOver.LastEpisodeName);
                    Preferences.Remove("EpisodeContinue_Upgrades_" + carryOver.LastEpisodeName);

                    Preferences.Commit();

                    Episode lastEpisode = GetEpisode(carryOver.LastEpisodeName);
                    if (lastEpisode != null && !string.IsNullOrEmpty(lastEpisode.NextEpisode)) {
                        // Redirect to next episode
                        Episode nextEpisode = GetEpisode(lastEpisode.NextEpisode);

                        carryOver.EpisodeName = lastEpisode.NextEpisode;
                        carryOver.LevelName = nextEpisode.FirstLevel;

                        // Start new level
                        LevelHandler handler = new LevelHandler(this, carryOver);

                        Scene.Current.DisposeLater();
                        Scene.SwitchTo(handler);
                    } else {
                        // Next episode not found...
                        ShowMainMenu(false);
                    }
                } else {
                    // Shouldn't happen...
                    ShowMainMenu(false);
                }
            } else if (carryOver.LevelName == ":credits") {
                // End of game

                if (!string.IsNullOrEmpty(carryOver.LastEpisodeName) && carryOver.LastEpisodeName != "unknown" && carryOver.PlayerCarryOvers.Length == 1) {
                    // ToDo: Implement time
                    Preferences.Set("EpisodeEnd_Time_" + carryOver.LastEpisodeName, (int)1);

                    // Remove existing continue data
                    Preferences.Remove("EpisodeContinue_Misc_" + carryOver.LastEpisodeName);
                    Preferences.Remove("EpisodeContinue_Level_" + carryOver.LastEpisodeName);
                    Preferences.Remove("EpisodeContinue_Ammo_" + carryOver.LastEpisodeName);
                    Preferences.Remove("EpisodeContinue_Upgrades_" + carryOver.LastEpisodeName);

                    Preferences.Commit();
                }

                // ToDo: Show credits

                PlayCinematics("ending", endOfStream => {
                    ShowMainMenu(false);
                });
            } else {
                if (!string.IsNullOrEmpty(carryOver.EpisodeName) && carryOver.EpisodeName != "unknown") {
                    Episode nextEpisode = GetEpisode(carryOver.EpisodeName);
                    if (nextEpisode.FirstLevel != carryOver.LevelName) {
                        // Save continue data, if it's not the first level of the episode
                        ref PlayerCarryOver player = ref carryOver.PlayerCarryOvers[0];

                        Preferences.Set("EpisodeContinue_Misc_" + carryOver.EpisodeName, new[] { (byte)player.Lives, (byte)carryOver.Difficulty, (byte)player.Type });
                        Preferences.Set("EpisodeContinue_Level_" + carryOver.EpisodeName, carryOver.LevelName);
                        if (player.Ammo != null) {
                            Preferences.Set("EpisodeContinue_Ammo_" + carryOver.EpisodeName, player.Ammo);
                        }

                        // Save WeaponUpgrades only if at least one of them is upgraded
                        if (player.WeaponUpgrades != null) {
                            for (int i = 0; i < player.WeaponUpgrades.Length; i++) {
                                if (player.WeaponUpgrades[i] != 0) {
                                    Preferences.Set("EpisodeContinue_Upgrades_" + carryOver.EpisodeName, player.WeaponUpgrades);
                                    break;
                                }
                            }
                        }

                        Preferences.Commit();
                    }
                }

                try {
                    LevelHandler handler = new LevelHandler(this, carryOver);

                    Scene.Current.DisposeLater();
                    Scene.SwitchTo(handler);
                } catch (Exception ex) {
                    Console.WriteLine("Cannot load level: " + ex);

                    ShowMainMenu(false);
                }
            }

            ContentResolver.Current.ReleaseUnreferencedResources();
        }

        public void PlayCinematics(string name, Action<bool> endCallback)
        {
            //System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;

            //ContentResolver.Current.ResetReferenceFlag();

            Scene.Current.DisposeLater();
            Scene.SwitchTo(new Cinematics(this, name, endCallback));

            //ContentResolver.Current.ReleaseUnreferencedResources();
        }

        private static Episode GetEpisode(string name)
        {
            JsonParser jsonParser = new JsonParser();

            string pathAbsolute = PathOp.Combine(DualityApp.DataDirectory, "Episodes", name, "Episode.res");
            if (FileOp.Exists(pathAbsolute)) {
                Episode json;
                using (Stream s = FileOp.Open(pathAbsolute, FileAccessMode.Read)) {
                    json = jsonParser.Parse<Episode>(s);
                }

                return json;
            } else {
                return null;
            }
        }

        private bool IsInstallationComplete()
        {
#if UNCOMPRESSED_CONTENT
            return true;
#else 
            return DirectoryOp.Exists(PathOp.Combine(DualityApp.DataDirectory, "Episodes")) &&
                DirectoryOp.Exists(PathOp.Combine(DualityApp.DataDirectory, "i18n")) &&
                //DirectoryOp.Exists(PathOp.Combine(DualityApp.DataDirectory, "Music")) &&
                DirectoryOp.Exists(PathOp.Combine(DualityApp.DataDirectory, "Tilesets")) &&
                FileOp.Exists(PathOp.Combine(DualityApp.DataDirectory, "Main.dz", "Animations", "Jazz", "idle.png")) &&
                FileOp.Exists(PathOp.Combine(DualityApp.DataDirectory, "Main.dz", "Metadata", "UI", "MainMenu.res"));
#endif
        }
    }
}