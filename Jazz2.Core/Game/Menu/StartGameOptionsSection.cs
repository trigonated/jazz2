﻿using System;
using Duality;
using Duality.Drawing;
using Duality.Input;
using Jazz2.Actors;
using Jazz2.Game.Structs;

namespace Jazz2.Game.Menu
{
    public class StartGameOptionsSection : MainMenuSection
    {
        private readonly string episodeName, levelName;

        private readonly string[] items = {
            "Character",
            "Difficulty",
            "Start Game"
        };

        private int selectedIndex = 2;
        private int selectedPlayerType;
        private int selectedDifficulty = 1;
        private float animation;

        public StartGameOptionsSection(string episodeName, string levelName)
        {
            this.episodeName = episodeName;
            this.levelName = levelName;
        }

        public override void OnShow(MainMenu root)
        {
            animation = 0f;
            base.OnShow(root);
        }

        public override void OnPaint(IDrawDevice device, Canvas c)
        {
            Vector2 center = device.TargetSize * 0.5f;
            center.Y *= 0.8f;

            string difficultyName;
            switch (selectedPlayerType) {
                default:
                case 0: difficultyName = "MenuDifficultyJazz"; break;
                case 1: difficultyName = "MenuDifficultySpaz"; break;
                case 2: difficultyName = "MenuDifficultyLori"; break;
            }

            api.DrawMaterial(c, "MenuDim", center.X * 0.36f, center.Y * 1.4f, Alignment.Center, ColorRgba.White, 30f, 40f);
            api.DrawMaterial(c, difficultyName, selectedDifficulty, center.X * 0.36f, center.Y * 1.4f + 3f, Alignment.Center, new ColorRgba(0f, 0.2f), 0.88f, 0.88f);
            api.DrawMaterial(c, difficultyName, selectedDifficulty, center.X * 0.36f, center.Y * 1.4f, Alignment.Center, ColorRgba.White, 0.88f, 0.88f);

            int charOffset = 0;
            for (int i = 0; i < items.Length; i++) {
                if (selectedIndex == i) {
                    float size = 0.5f + MainMenu.EaseOutElastic(animation) * 0.6f;

                    api.DrawStringShadow(device, ref charOffset, items[i], center.X, center.Y,
                        Alignment.Center, null, size, 0.7f, 1.1f, 1.1f, charSpacing: 0.9f);
                } else {
                    api.DrawString(device, ref charOffset, items[i], center.X, center.Y, Alignment.Center,
                        ColorRgba.TransparentBlack, 0.9f);
                }

                if (i == 0) {
                    string[] playerTypes = { "Jazz", "Spaz", "Lori" };
                    ColorRgba[] playerColors = {
                        new ColorRgba(0.2f, 0.45f, 0.2f, 0.5f),
                        new ColorRgba(0.45f, 0.27f, 0.22f, 0.5f),
                        new ColorRgba(0.5f, 0.45f, 0.22f, 0.5f)
                    };
                    for (int j = 0; j < playerTypes.Length; j++) {
                        if (selectedPlayerType == j) {
                            api.DrawStringShadow(device, ref charOffset, playerTypes[j], center.X + (j - 1) * 100f, center.Y + 28f, Alignment.Center,
                                /*null*/playerColors[j], 0.9f, 0.4f, 0.55f, 0.55f, 8f, 0.9f);
                        } else {
                            api.DrawString(device, ref charOffset, playerTypes[j], center.X + (j - 1) * 100f, center.Y + 28f, Alignment.Center,
                                ColorRgba.TransparentBlack, 0.8f, charSpacing: 0.9f);
                        }
                    }

                    api.DrawStringShadow(device, ref charOffset, "<", center.X - (100f + 40f), center.Y + 28f, Alignment.Center,
                        ColorRgba.TransparentBlack, 0.7f);
                    api.DrawStringShadow(device, ref charOffset, ">", center.X + (100f + 40f), center.Y + 28f, Alignment.Center,
                        ColorRgba.TransparentBlack, 0.7f);
                } else if (i == 1) {
                    string[] difficultyTypes = { "Easy", "Medium", "Hard" };
                    for (int j = 0; j < difficultyTypes.Length; j++) {
                        if (selectedDifficulty == j) {
                            api.DrawStringShadow(device, ref charOffset, difficultyTypes[j], center.X + (j - 1) * 100f, center.Y + 28f, Alignment.Center,
                                null, 0.9f, 0.4f, 0.55f, 0.55f, 8f, 0.9f);
                        } else {
                            api.DrawString(device, ref charOffset, difficultyTypes[j], center.X + (j - 1) * 100f, center.Y + 28f, Alignment.Center,
                                ColorRgba.TransparentBlack, 0.8f, charSpacing: 0.9f);
                        }
                    }

                    api.DrawStringShadow(device, ref charOffset, "<", center.X - (100f + 40f), center.Y + 28f, Alignment.Center,
                        ColorRgba.TransparentBlack, 0.7f);
                    api.DrawStringShadow(device, ref charOffset, ">", center.X + (100f + 40f), center.Y + 28f, Alignment.Center,
                        ColorRgba.TransparentBlack, 0.7f);
                }

                center.Y += 70f;
            }
        }

        public override void OnUpdate()
        {
            if (animation < 1f) {
                animation = Math.Min(animation + Time.TimeMult * 0.016f, 1f);
            }

            if (DualityApp.Keyboard.KeyHit(Key.Enter)) {
                if (selectedIndex == 2) {
                    api.PlaySound("MenuSelect", 0.5f);
                    api.SwitchToLevel(new InitLevelData(
                        episodeName,
                        levelName,
                        (GameDifficulty.Easy + selectedDifficulty),
                        (PlayerType.Jazz + selectedPlayerType)
                    ));
                }
            } else if (DualityApp.Keyboard.KeyHit(Key.Left)) {
                if (selectedIndex == 0) {
                    if (selectedPlayerType > 0) {
                        selectedPlayerType--;
                    }
                } else if (selectedIndex == 1) {
                    if (selectedDifficulty > 0) {
                        selectedDifficulty--;
                    }
                }
            } else if (DualityApp.Keyboard.KeyHit(Key.Right)) {
                if (selectedIndex == 0) {
                    if (selectedPlayerType < 3 - 1) {
                        selectedPlayerType++;
                    }
                } else if (selectedIndex == 1) {
                    if (selectedDifficulty < 3 - 1) {
                        selectedDifficulty++;
                    }
                }
            } else if (DualityApp.Keyboard.KeyHit(Key.Up)) {
                api.PlaySound("MenuSelect", 0.4f);
                animation = 0f;
                if (selectedIndex > 0) {
                    selectedIndex--;
                } else {
                    selectedIndex = items.Length - 1;
                }
            } else if (DualityApp.Keyboard.KeyHit(Key.Down)) {
                api.PlaySound("MenuSelect", 0.4f);
                animation = 0f;
                if (selectedIndex < items.Length - 1) {
                    selectedIndex++;
                } else {
                    selectedIndex = 0;
                }
            } else if (DualityApp.Keyboard.KeyHit(Key.Escape)) {
                api.PlaySound("MenuSelect", 0.5f);
                api.LeaveSection(this);
            }
        }
    }
}