﻿using System;
using Duality;
using Duality.Drawing;
using Jazz2.Game.UI.Menu.InGame;
using Jazz2.Storage;
using static Jazz2.SettingsCache;

namespace Jazz2.Game.UI.Menu.Settings
{
    public class EnhancementsSection : MenuSectionWithControls
    {
        private ChoiceControl reduxMode;
        private ChoiceControl enableLedgeClimb;

        public override void OnShow(IMenuContainer root)
        {
            base.OnShow(root);

            bool enableReduxMode = Preferences.Get<bool>("ReduxMode", true);

            reduxMode = new ChoiceControl(api, "menu/settings/enhancements/redux mode".T(), enableReduxMode ? 1 : 0, "disabled".T(), "enabled".T());
            enableLedgeClimb = new ChoiceControl(api, "menu/settings/enhancements/ledge climb".T(), EnableLedgeClimb ? 1 : 0, "disabled".T(), "enabled".T());

            reduxMode.IsEnabled = !(root is InGameMenu);

            controls = new MenuControlBase[] {
                reduxMode,
                enableLedgeClimb
            };
        }

        public override void OnPaint(Canvas canvas, Rect view)
        {
            IDrawDevice device = canvas.DrawDevice;

            Vector2 center = device.TargetSize * 0.5f;

            const float topLine = 216f;
            float bottomLine = device.TargetSize.Y - 42;
            api.DrawMaterial("MenuDim", center.X, (topLine + bottomLine) * 0.5f, Alignment.Center, ColorRgba.White, 55f, (bottomLine - topLine) * 0.063f, new Rect(0f, 0.3f, 1f, 0.4f));

            api.DrawMaterial("MenuLine", 0, center.X, topLine, Alignment.Center, ColorRgba.White, 1.6f);
            api.DrawMaterial("MenuLine", 1, center.X, bottomLine, Alignment.Center, ColorRgba.White, 1.6f);

            int charOffset = 0;
            api.DrawStringShadow(ref charOffset, "menu/settings/enhancements".T(), center.X, 110f,
                Alignment.Center, new ColorRgba(0.5f, 0.5f), 0.9f, 0.4f, 0.6f, 0.6f, 8f, charSpacing: 0.88f);

            api.DrawStringShadow(ref charOffset, "menu/settings/enhancements/info".T(), center.X, 166f,
                Alignment.Center, new ColorRgba(0.5f, 0.5f), 0.76f, 0.4f, 0.6f, 0.6f, 8f, charSpacing: 0.88f, lineSpacing: 1.3f);

            if (controls == null) {
                return;
            }

            Vector2 size = device.TargetSize;

            Vector2 pos = size * 0.5f;
            pos.Y *= 1.16f;

            float maxVisibleItemsFloat = (size.Y - pos.Y - 40f) / 45f;
            maxVisibleItems = (int)maxVisibleItemsFloat;

            if (maxVisibleItems > controls.Length) {
                maxVisibleItems = Math.Max(controls.Length, 3);
            }

            pos.Y += (maxVisibleItemsFloat - maxVisibleItems) / maxVisibleItems * 45f;

            for (int i = 0; i < maxVisibleItems; i++) {
                int idx = i;
                if (idx >= controls.Length) {
                    break;
                }

                controls[idx].OnDraw(canvas, ref pos, selectedIndex == idx, animation);
            }
        }

        public override void OnHide(bool isRemoved)
        {
            Commit();

            base.OnHide(isRemoved);
        }

        private void Commit()
        {
            bool enableReduxMode = (reduxMode.SelectedIndex == 1);
            Preferences.Set("ReduxMode", enableReduxMode);

            EnableLedgeClimb = (enableLedgeClimb.SelectedIndex == 1);
            Preferences.Set("EnableLedgeClimb", EnableLedgeClimb);

            Preferences.Commit();
        }
    }
}