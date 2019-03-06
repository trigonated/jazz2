﻿using System;
using System.IO;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Jazz2.Android;
using Jazz2.Game;
using Path = System.IO.Path;

namespace Duality.Android
{
    [Activity(
        MainLauncher = true,
        Icon = "@mipmap/ic_launcher",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden,
        ScreenOrientation = ScreenOrientation.UserLandscape,
        LaunchMode = LaunchMode.SingleInstance
    )]
    public class MainActivity : Activity
    {
        private static WeakReference<MainActivity> weakActivity;

        public static MainActivity Current
        {
            get
            {
                MainActivity activity;
                weakActivity.TryGetTarget(out activity);
                return activity;
            }
        }


        internal InnerView InnerView;

        private VideoView backgroundVideo;
        private Button retryButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            weakActivity = new WeakReference<MainActivity>(this);

            CrashHandlerActivity.Register(this);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            try {
                View decorView = Window.DecorView;
                decorView.SystemUiVisibility |= (StatusBarVisibility)SystemUiFlags.LayoutStable;
                decorView.SystemUiVisibility |= (StatusBarVisibility)SystemUiFlags.LayoutFullscreen;
                //decorView.SystemUiVisibility |= (StatusBarVisibility)SystemUiFlags.Immersive;

                // Minimal supported SDK is already 18
                //if ((int)Build.VERSION.SdkInt < 18)
                //    RequestedOrientation = ScreenOrientation.SensorLandscape;

                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            } catch /*(Exception ex)*/ {
#if DEBUG
                throw;
#endif
            }

            TryInit();
        }

        protected override void OnDestroy()
        {
            weakActivity.SetTarget(null);

            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (InnerView != null) {
                InnerView.Pause();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (InnerView != null) {
                InnerView.Resume();
            }
            if (backgroundVideo != null) {
                backgroundVideo.Start();
            }
        }

        private void TryInit()
        {
            if (!CheckAppPermissions()) {
                ShowInfoScreen("Access denied", "You have to grant file access permissions to&nbsp;continue!");
                return;
            }

            string rootPath = Backend.Android.NativeFileSystem.FindRootPath();
            if (rootPath == null) {
                var storageList = Backend.Android.NativeFileSystem.GetStorageList();
                if (storageList.Count == 0) {
                    ShowInfoScreen("Content files not found", "No storage is accessible.");
                    return;
                }

                var found = storageList.Find(storage => storage.Path == "/storage/emulated/0");
                if (found.Path == null) {
                    found = storageList[0];
                }

                ShowInfoScreen("Content files not found", "Content should be placed in&nbsp;<u>" + Path.Combine(found.Path, "Android", "Data", Application.Context.PackageName, "Content") + "/…</u> or&nbsp;in&nbsp;other compatible path.");
                return;
            }

            if (!File.Exists(Path.Combine(rootPath, "Content", "Main.dz"))) {
                ShowInfoScreen("Content files not found", "Content should be placed in&nbsp;<u>" + Path.Combine(rootPath, "Content") + "/…</u><br>It includes <b>Main.dz</b> file and <b>Episodes</b>, <b>Internal</b>, <b>Music</b>, <b>Shaders</b>, <b>Tilesets</b> directories.");
                return;
            }

            RunGame();
        }

        private bool CheckAppPermissions()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M) {
                return true;
            }
            
            if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) != Permission.Granted
                && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                RequestPermissions(permissions, 1);
                return false;
            } else {
                return true;
            }
        }
        
        private void ShowInfoScreen(string header, string content)
        {
            content += "<br><br><small>If you have any issues, report it to developers.<br><a href=\"https://github.com/deathkiller/jazz2\">https://github.com/deathkiller/jazz2</a></small>";

            try {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                    Window.SetStatusBarColor(new Color(0x30000000));
                }
            } catch {
                // Nothing to do...
            }

            if (backgroundVideo == null || retryButton == null) {
                SetContentView(Jazz2.Android.Resource.Layout.activity_info);

                backgroundVideo = FindViewById<VideoView>(Jazz2.Android.Resource.Id.background_video);
                backgroundVideo.SetVideoURI(global::Android.Net.Uri.Parse("android.resource://" + PackageName + "/raw/logo"));
                backgroundVideo.Prepared += OnVideoViewPrepared;
                backgroundVideo.Start();

                retryButton = FindViewById<Button>(Jazz2.Android.Resource.Id.retry_button);
                retryButton.Click += OnRetryButtonClick;

                TextView versionView = FindViewById<TextView>(Jazz2.Android.Resource.Id.version);
                versionView.Text = "v" + App.AssemblyVersion;
            }

            TextView headerView = FindViewById<TextView>(Jazz2.Android.Resource.Id.header);
            headerView.Text = header;

            TextView contentView = FindViewById<TextView>(Jazz2.Android.Resource.Id.content);
            contentView.MovementMethod = LinkMovementMethod.Instance;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N) {
                contentView.TextFormatted = Html.FromHtml(content, FromHtmlOptions.ModeLegacy);
            } else {
                contentView.TextFormatted = Html.FromHtml(content);
            }
        }

        private void OnRetryButtonClick(object sender, EventArgs e)
        {
            TryInit();
        }

        private void OnVideoViewPrepared(object sender, EventArgs e)
        {
            ((global::Android.Media.MediaPlayer)sender).Looping = true;
        }

        private void RunGame()
        {
            try {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                    Window.SetStatusBarColor(new Color(0));
                }
            } catch {
                // Nothing to do...
            }

            if (backgroundVideo != null) {
                backgroundVideo.Prepared -= OnVideoViewPrepared;
                backgroundVideo = null;
            }
            if (retryButton != null) {
                retryButton.Click -= OnRetryButtonClick;
                retryButton = null;
            }

            // Create our OpenGL view and show it
            InnerView = new InnerView(this);
            SetContentView(InnerView);
        }
    }
}