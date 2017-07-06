﻿using Duality;
using Duality.Drawing;
using Duality.Resources;

namespace Jazz2.Game.Menu
{
    public class InGameMenuRenderSetup : RenderSetup
    {
        public static Point2 TargetSize = new Point2(defaultWidth, defaultHeight);

        private const int defaultWidth = 720, defaultHeight = 405;
        private Vector2 lastImageSize;

        private readonly ContentRef<DrawTechnique> resizeShader;

        private Texture finalTexture;
        private RenderTarget finalTarget;

        public InGameMenuRenderSetup()
        {
            // Shaders
            switch (Settings.Resize) {
                default:
                case Settings.ResizeMode.None:
                    resizeShader = DrawTechnique.Solid;
                    break;
                case Settings.ResizeMode.HQ2x:
                    resizeShader = ContentResolver.Current.RequestShader("ResizeHQ2x");
                    break;
                case Settings.ResizeMode.xBRZ:
                    resizeShader = ContentResolver.Current.RequestShader("Resize3xBRZ");
                    break;
            }

            finalTexture = new Texture(null, TextureSizeMode.NonPowerOfTwo, TextureMagFilter.Nearest, TextureMinFilter.Nearest);

            finalTarget = new RenderTarget(AAQuality.Off, false, finalTexture);

            
            // Render steps
            AddRenderStep(RenderStepPosition.Last, new RenderStep {
                MatrixMode = RenderMatrix.ScreenSpace,
                VisibilityMask = VisibilityFlag.All,
                ClearFlags = ClearFlag.None,

                Output = finalTarget
            });

            AddRenderStep(RenderStepPosition.Last, new RenderStep {
                Id = "Final",

                MatrixMode = RenderMatrix.ScreenSpace,
                VisibilityMask = VisibilityFlag.None
            });
        }

        protected override void OnDisposing(bool manually)
        {
            base.OnDisposing(manually);

            Disposable.Free(ref finalTarget);
            Disposable.Free(ref finalTexture);
        }

        protected override void OnRenderPointOfView(Scene scene, DrawDevice drawDevice, Rect viewportRect, Vector2 imageSize)
        {
            if (lastImageSize != imageSize) {
                lastImageSize = imageSize;

                const float defaultRatio = (float)defaultWidth / defaultHeight;
                float currentRatio = imageSize.X / imageSize.Y;

                int width, height;
                if (currentRatio > defaultRatio) {
                    width = MathF.Min(defaultWidth, (int)imageSize.X);
                    height = (int)(width / currentRatio);
                } else if (currentRatio < defaultRatio) {
                    height = MathF.Min(defaultHeight, (int)imageSize.Y);
                    width = (int)(height * currentRatio);
                } else {
                    width = MathF.Min(defaultWidth, (int)imageSize.X);
                    height = MathF.Min(defaultHeight, (int)imageSize.Y);
                }

                TargetSize = new Point2(width, height);

                ResizeRenderTarget(finalTarget, TargetSize);
            }

            base.OnRenderPointOfView(scene, drawDevice, viewportRect, imageSize);
        }

        protected override void OnRenderSingleStep(RenderStep step, Scene scene, DrawDevice drawDevice)
        {
            if (step.Id == "Final") {
                ProcessFinalStep(drawDevice);
            } else {
                base.OnRenderSingleStep(step, scene, drawDevice);
            }
        }

        private void ProcessFinalStep(DrawDevice drawDevice)
        {
            BatchInfo material = new BatchInfo(resizeShader, ColorRgba.White, finalTexture);
            material.SetUniform("mainTexSize", (float)finalTexture.ContentWidth, (float)finalTexture.ContentHeight);
            Blit(drawDevice, material, drawDevice.ViewportRect);
            
        }

        private static void Blit(DrawDevice device, BatchInfo source, RenderTarget target)
        {
            device.Target = target;
            device.TargetSize = target.Size;
            device.ViewportRect = new Rect(target.Size);

            device.PrepareForDrawcalls();
            device.AddFullscreenQuad(source, TargetResize.Stretch);
            device.Render();
        }

        private static void Blit(DrawDevice device, BatchInfo source, Rect screenRect)
        {
            device.Target = null;
            device.TargetSize = screenRect.Size;
            device.ViewportRect = screenRect;

            device.PrepareForDrawcalls();
            device.AddFullscreenQuad(source, TargetResize.Stretch);
            device.Render();
        }
    }
}