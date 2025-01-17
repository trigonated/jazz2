﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Duality.Input;
using Duality.IO;
using Jazz2;
using Jazz2.Game;
using OpenTK;

namespace Duality.Backend.DefaultOpenTK
{
    public class DefaultOpenTKBackendPlugin : CorePlugin
    {
        private static bool openTKInitialized;
        private static Thread mainThread;
        private double lastInputDeviceUpdate;

        protected override void InitPlugin()
        {
            base.InitPlugin();

            mainThread = Thread.CurrentThread;

            // Initialize OpenTK, if not done yet
            InitOpenTK();

            // Initially check for available input devices
            this.DetectInputDevices();
        }

        protected override void OnAfterUpdate()
        {
            base.OnAfterUpdate();

            // Periodically check for global / non-windowbound input devices
            if (Time.MainTimer.TotalSeconds - this.lastInputDeviceUpdate > 5.0f) {
                this.DetectInputDevices();
            }
        }

        protected override void OnDisposePlugin()
        {
            base.OnDisposePlugin();

            foreach (GamepadInput gamepad in DualityApp.Gamepads.ToArray()) {
                if (gamepad.Source is GlobalGamepadInputSource)
                    DualityApp.Gamepads.RemoveSource(gamepad.Source);
            }
            foreach (JoystickInput joystick in DualityApp.Joysticks.ToArray()) {
                if (joystick.Source is GlobalJoystickInputSource)
                    DualityApp.Joysticks.RemoveSource(joystick.Source);
            }
        }

        private void DetectInputDevices()
        {
            this.lastInputDeviceUpdate = Time.MainTimer.TotalSeconds;

            GlobalGamepadInputSource.UpdateAvailableDecives(DualityApp.Gamepads);
            GlobalJoystickInputSource.UpdateAvailableDecives(DualityApp.Joysticks);
        }

        public static void InitOpenTK()
        {
            if (openTKInitialized) return;
            openTKInitialized = true;

            Assembly execAssembly = Assembly.GetEntryAssembly() ?? typeof(DualityApp).Assembly;
            string execAssemblyDir = PathOp.GetFullPath(PathOp.GetDirectoryName(execAssembly.Location));

            bool isWindows =
                Environment.OSVersion.Platform == PlatformID.Win32NT ||
                Environment.OSVersion.Platform == PlatformID.Win32S ||
                Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                Environment.OSVersion.Platform == PlatformID.WinCE;
            bool genericFolderSDL = isWindows && !FileOp.Exists("SDL2.dll") && !FileOp.Exists(PathOp.Combine(execAssemblyDir, "SDL2.dll"));

            ToolkitOptions options = new ToolkitOptions {
                // Prefer the native backend in the editor, because it supports GLControl. SDL doesn't.
                // Also, never use SDL if it isn't in the local game folder, because it might be in PATH on some machines.
                Backend = (/*inEditor ||*/ genericFolderSDL) ? PlatformBackend.PreferNative : PlatformBackend.Default,

                // Disable High Resolution support in the editor, because it's not DPI-Aware
                EnableHighResolution = /*!inEditor*/true
            };

            Log.Write(LogType.Info, "Initializing OpenTK...");
            Log.PushIndent();
            Log.Write(LogType.Info,
                "Platform Backend: {0}" + Environment.NewLine +
                "EnableHighResolution: {1}",
                options.Backend,
                options.EnableHighResolution);
            Log.PopIndent();

            Toolkit.Init(options);
        }
        /// <summary>
        /// Guards the calling method agains being called from a thread that is not the main thread.
        /// Use this only at critical code segments that are likely to be called from somewhere else than the main thread
        /// but aren't allowed to.
        /// </summary>
        /// <param name="backend"></param>
        /// <param name="silent"></param>
        /// <returns>True if everyhing is allright. False if the guarded state has been violated.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool GuardSingleThreadState(bool silent = false, [CallerMemberName] string callerInfoMember = null)
        {
            if (Thread.CurrentThread != mainThread) {
                if (!silent) {
                    Log.Write(LogType.Error,
                        "Method {0} isn't allowed to be called from a Thread that is not the main Thread.",
                        callerInfoMember);
                }
                return false;
            }
            return true;
        }
    }
}
