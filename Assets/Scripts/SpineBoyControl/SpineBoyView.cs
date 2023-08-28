using Spine.Unity.Examples;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animation = Spine.Animation;
using AnimationState = Spine.AnimationState;

namespace Net 
{
    public class SpineBoyView : MonoBehaviour
    {
        #region Inspector
        [Header("Components")]
        public SpineBoyModel model;
        public SkeletonAnimation skeletonAnimation;

        public AnimationReferenceAsset run, idle, aim, shoot, jump;
        public EventDataReferenceAsset footstepEvent;

        [Header("Audio")]
        public float footstepPitchOffset = 0.2f;
        public float gunsoundPitchOffset = 0.13f;
        public AudioSource footstepSource, gunSource, jumpSource;

        [Header("Effects")]
        public ParticleSystem gunParticles;
        #endregion

        SpineBeginnerBodyState previousViewState;

        void Start()
        {
            if (skeletonAnimation == null) return;
            model.ShootEvent += PlayShoot;
            model.StartAimEvent += StartPlayingAim;
            model.StopAimEvent += StopPlayingAim;
            skeletonAnimation.AnimationState.Event += HandleEvent;
        }

        void HandleEvent(Spine.TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data == footstepEvent.EventData)
                PlayFootstepSound();
        }

        void Update()
        {
            if (skeletonAnimation == null) return;
            if (model == null) return;

            if ((skeletonAnimation.skeleton.ScaleX < 0) != model.facingLeft)
            {  // Detect changes in model.facingLeft
                Turn(model.facingLeft);
            }

            // Detect changes in model.state
            SpineBeginnerBodyState currentModelState = model.state;

            if (previousViewState != currentModelState)
            {
                PlayNewStableAnimation();
            }

            previousViewState = currentModelState;
        }

        void PlayNewStableAnimation()
        {
            SpineBeginnerBodyState newModelState = model.state;
            Animation nextAnimation;

            // Add conditionals to not interrupt transient animations.
            if (previousViewState == SpineBeginnerBodyState.Jumping && newModelState != SpineBeginnerBodyState.Jumping)
            {
                PlayFootstepSound();
            }

            if (newModelState == SpineBeginnerBodyState.Jumping)
            {
                jumpSource.Play();
                nextAnimation = jump;
            }
            else
            {
                if (newModelState == SpineBeginnerBodyState.Running)
                {
                    nextAnimation = run;
                }
                else
                {
                    nextAnimation = idle;
                }
            }

            skeletonAnimation.AnimationState.SetAnimation(0, nextAnimation, true);
        }

        void PlayFootstepSound()
        {
            footstepSource.Play();
            footstepSource.pitch = GetRandomPitch(footstepPitchOffset);
        }

        [ContextMenu("Check Tracks")]
        void CheckTracks()
        {
            AnimationState state = skeletonAnimation.AnimationState;
            Debug.Log(state.GetCurrent(0));
            Debug.Log(state.GetCurrent(1));
        }

        #region Transient Actions
        public void PlayShoot()
        {
            // Play the shoot animation on track 1.
            TrackEntry shootTrack = skeletonAnimation.AnimationState.SetAnimation(1, shoot, false);
            shootTrack.AttachmentThreshold = 1f;
            shootTrack.MixDuration = 0f;
            skeletonAnimation.state.AddEmptyAnimation(1, 0.5f, 0.1f);

            // Play the aim animation on track 2 to aim at the mouse target.
            TrackEntry aimTrack = skeletonAnimation.AnimationState.SetAnimation(2, aim, false);
            aimTrack.AttachmentThreshold = 1f;
            aimTrack.MixDuration = 0f;
            skeletonAnimation.state.AddEmptyAnimation(2, 0.5f, 0.1f);

            gunSource.pitch = GetRandomPitch(gunsoundPitchOffset);
            gunSource.Play();
            //gunParticles.randomSeed = (uint)Random.Range(0, 100);
            gunParticles.Play();
        }

        public void StartPlayingAim()
        {
            // Play the aim animation on track 2 to aim at the mouse target.
            TrackEntry aimTrack = skeletonAnimation.AnimationState.SetAnimation(2, aim, true);
            aimTrack.AttachmentThreshold = 1f;
            aimTrack.MixDuration = 0f;
        }

        public void StopPlayingAim()
        {
            skeletonAnimation.state.AddEmptyAnimation(2, 0.5f, 0.1f);
        }

        public void Turn(bool facingLeft)
        {
            skeletonAnimation.Skeleton.ScaleX = facingLeft ? -1f : 1f;
            // Maybe play a transient turning animation too, then call ChangeStableAnimation.
        }
        #endregion

        #region Utility
        public float GetRandomPitch(float maxPitchOffset)
        {
            return 1f + Random.Range(-maxPitchOffset, maxPitchOffset);
        }
        #endregion
    }
}

