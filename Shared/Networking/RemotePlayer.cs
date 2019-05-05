﻿using Duality;
using Jazz2.Actors;
using Jazz2.Game.Structs;

namespace Jazz2.Game.Multiplayer
{
    public class RemotePlayer : ActorBase
    {
        private PlayerType playerType;

        public int Index;

        public override void OnAttach(ActorInstantiationDetails details)
        {
            base.OnAttach(details);

            playerType = (PlayerType)details.Params[0];
            Index = details.Params[1];

            health = int.MaxValue;

            switch (playerType) {
                case PlayerType.Jazz:
                    RequestMetadata("Interactive/PlayerJazz");
                    break;
                case PlayerType.Spaz:
                    RequestMetadata("Interactive/PlayerSpaz");
                    break;
                case PlayerType.Lori:
                    RequestMetadata("Interactive/PlayerLori");
                    break;
                case PlayerType.Frog:
                    RequestMetadata("Interactive/PlayerFrog");
                    break;
            }

            SetAnimation(AnimState.Fall);

            collisionFlags = CollisionFlags.CollideWithOtherActors;
        }

        public void UpdateFromServer(Vector3 pos, Vector2 speed, AnimState animState, float animTime, bool isFacingLeft)
        {
            Transform.Pos = pos;

            speedX = speed.X;
            speedY = speed.Y;

            if (availableAnimations != null) {
                if (currentAnimationState != animState) {
                    SetAnimation(animState);
                }

                renderer.AnimTime = animTime;
                IsFacingLeft = isFacingLeft;
            }
        }
    }
}