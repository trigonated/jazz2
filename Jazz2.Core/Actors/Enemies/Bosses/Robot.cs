﻿using System.Collections.Generic;
using Duality;
using Jazz2.Actors.Enemies;
using Jazz2.Game.Structs;

namespace Jazz2.Actors.Bosses
{
    public class Robot : EnemyBase
    {
        private const int StateTransition = -1;
        private const int StateWaiting = 0;
        private const int StateRunning1 = 1;
        private const int StateRunning2 = 2;
        private const int StatePreparingToRun = 3;

        private int state = StateWaiting;
        private float stateTime;
        private int shots;

        public override void OnAttach(ActorInstantiationDetails details)
        {
            base.OnAttach(details);

            SetHealthByDifficulty(100);
            scoreValue = 2000;

            RequestMetadata("Boss/Robot");
            SetAnimation(AnimState.Idle);
        }

        public void Activate()
        {
            FollowNearestPlayer(StateRunning1, MathF.Rnd.NextFloat(20, 40));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (frozenTimeLeft > 0) {
                return;
            }

            switch (state) {
                case StateRunning1: {
                    if (stateTime <= 0f) {
                        FollowNearestPlayer(
                            MathF.Rnd.NextFloat() < 0.65f ? StateRunning1 : StateRunning2,
                            MathF.Rnd.NextFloat(10, 30));
                    }
                    break;
                }

                case StateRunning2: {
                    if (stateTime <= 0f) {
                        speedX = 0f;

                        state = StateTransition;
                        PlaySound("ATTACK_START");
                        SetAnimation(AnimState.Idle);
                        SetTransition((AnimState)1073741824, false, delegate {
                            shots = MathF.Rnd.Next(1, 4);
                            Shoot();
                        });
                    }
                    break;
                }

                case StatePreparingToRun: {
                    if (stateTime <= 0f) {
                        FollowNearestPlayer(
                            StateRunning1,
                            MathF.Rnd.NextFloat(10, 30));
                    }
                    break;
                }
            }

            stateTime -= Time.TimeMult;
        }

        protected override bool OnPerish(ActorBase collider)
        {
            CreateParticleDebris();

            string[] shrapnels = new[] {
                "SHRAPNEL_1", "SHRAPNEL_2", "SHRAPNEL_3",
                "SHRAPNEL_4", "SHRAPNEL_5", "SHRAPNEL_6",
                "SHRAPNEL_7", "SHRAPNEL_8", "SHRAPNEL_9"
            };
            for (int i = 0; i < 6; i++) {
                CreateSpriteDebris(MathF.Rnd.OneOf(shrapnels), 1);
            }

            api.PlayCommonSound(this, "COMMON_SPLAT");

            return base.OnPerish(collider);
        }

        protected override void OnHealthChanged(ActorBase collider)
        {
            base.OnHealthChanged(collider);

            string[] shrapnels = new[] {
                "SHRAPNEL_1", "SHRAPNEL_2", "SHRAPNEL_3",
                "SHRAPNEL_4", "SHRAPNEL_5", "SHRAPNEL_6",
                "SHRAPNEL_7", "SHRAPNEL_8", "SHRAPNEL_9"
            };
            int n = MathF.Rnd.Next(1, 3);
            for (int i = 0; i < n; i++) {
                CreateSpriteDebris(MathF.Rnd.OneOf(shrapnels), 1);
            }

            PlaySound("SHRAPNEL");
        }

        private void FollowNearestPlayer(int newState, float time)
        {
            bool found = false;
            Vector3 pos = Transform.Pos;
            Vector3 targetPos = new Vector3(float.MaxValue, float.MaxValue, 0f);

            List<Player> players = api.Players;
            for (int i = 0; i < players.Count; i++) {
                Vector3 newPos = players[i].Transform.Pos;
                if ((pos - newPos).Length < (pos - targetPos).Length) {
                    targetPos = newPos;
                    found = true;
                }
            }

            if (found) {
                state = newState;
                stateTime = time;

                isFacingLeft = (targetPos.X < pos.X);
                
                float mult = MathF.Rnd.NextFloat(0.6f, 0.9f);
                speedX = (isFacingLeft ? -3f : 3f) * mult;
                renderer.AnimDuration = currentAnimation.FrameDuration / mult;

                PlaySound("RUN");
                SetAnimation(AnimState.Run);
            }
        }

        private void Shoot()
        {
            SpikeBall spikeBall = new SpikeBall();
            spikeBall.OnAttach(new ActorInstantiationDetails {
                Api = api,
                Pos = Transform.Pos + new Vector3(0f, -32f, 0f),
                Params = new[] { (ushort)(isFacingLeft ? 1 : 0) }
            });
            api.AddActor(spikeBall);

            shots--;

            PlaySound("ATTACK");
            SetTransition((AnimState)1073741825, false, delegate {
                if (shots > 0) {
                    PlaySound("ATTACK_START_SHUTTER");
                    Shoot();
                } else {
                    Run();
                }
            });
        }

        private void Run()
        {
            PlaySound("ATTACK_END");
            SetTransition((AnimState)1073741826, false, delegate {
                state = StatePreparingToRun;
                stateTime = 10f;
            });
        }

        // ToDo: Implement oversaturated light
        private class SpikeBall : EnemyBase
        {
            public override void OnAttach(ActorInstantiationDetails details)
            {
                base.OnAttach(details);

                isFacingLeft = (details.Params[0] != 0);

                canBeFrozen = false;
                health = int.MaxValue;

                speedX = (isFacingLeft ? -8f : 8f);

                RequestMetadata("Boss/Robot");
                SetAnimation((AnimState)1073741827);

                OnUpdateHitbox();
            }

            protected override void OnUpdate()
            {
                base.OnUpdate();
            }

            protected override void OnUpdateHitbox()
            {
                UpdateHitbox(8, 8);
            }

            protected override bool OnPerish(ActorBase collider)
            {
                Explosion.Create(api, Transform.Pos, Explosion.SmallDark);

                return base.OnPerish(collider);
            }

            public override void HandleCollision(ActorBase other)
            {
            }

            protected override void OnHitFloorHook()
            {
                DecreaseHealth(int.MaxValue);
            }

            protected override void OnHitWallHook()
            {
                DecreaseHealth(int.MaxValue);
            }

            protected override void OnHitCeilingHook()
            {
                DecreaseHealth(int.MaxValue);
            }
        }
    }
}