﻿using System;
using System.Collections;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using UnityEngine;
using AnimationState = Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts.AnimationState;

namespace Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts
{
    public class CharacterControls : MonoBehaviour {
        public bool isGoblinEnemy;
        public Character Character;
        public CharacterController Controller; // https://docs.unity3d.com/ScriptReference/CharacterController.html
        public float RunSpeed = 1f;
        public float JumpSpeed = 3f;
        public float CrawlSpeed = 0.25f;
        public float Gravity = -0.2f;
        public ParticleSystem MoveDust;
        public ParticleSystem JumpDust;

        Coroutine GoblinAnimID;

        private Vector3 _motion = Vector3.zero;
        private int _inputX, _inputY;
        private float _activityTime;
        
        public void Awake() {
            if(!isGoblinEnemy)
                SpawnAnim();
        }

        private void OnEnable() {
            if(isGoblinEnemy)
                Character.SetState(AnimationState.Walking);
        }

        private IEnumerator BounceAnim() {
            Character.SetState(AnimationState.Jumping);
            yield return Util.Time0_15;
            Character.SetState(AnimationState.Idle);
            JumpDust.Play(true);
            yield return Util.Time0_15;
        }

        /// <summary>
        /// 登場アニメー（一発）
        /// </summary>
        public void SpawnAnim() => StartCoroutine(CoSpawnAnim());
        IEnumerator CoSpawnAnim() {
            yield return BounceAnim();
        }

        /// <summary>
        /// ゴブリン採掘アニメー（ループ）
        /// </summary>
        public void MiningAnim(int goblinLv) {
            Debug.Log("MiningAnim():: START");
            if(GoblinAnimID != null) StopCoroutine(GoblinAnimID);
            GoblinAnimID = StartCoroutine(CoMiningAnim(goblinLv));
        }
        IEnumerator CoMiningAnim(int lv) {
            //* ゴブリンLVによって打つ速度早く
            WaitForSeconds waitSec = (lv == 0)? Util.Time1 
                : (lv == 1)? Util.Time0_8
                : (lv == 2)? Util.Time0_65
                : (lv == 3)? Util.Time0_5
                : (lv == 4)? Util.Time0_4
                : (lv == 5)? Util.Time0_3
                : Util.Time0_2;

            while(true) {
                Debug.Log($"MiningAnim():: SLASH:: LV= {lv}");
                Character.SetState(AnimationState.Slash);

                //* 他のPOPUPが開いていたら、この音をしない
                if(!HM._.hui.IsActivePopUp)
                    SM._.SfxPlay(SM.SFX.MetalHitSFX);

                HM._.wsm.MetalHitEF.Play();
                yield return waitSec;
                Character.SetState(AnimationState.Idle);
                yield return waitSec;
            }
        }

        /// <summary>
        /// 幸せアニメー（ループ）
        /// </summary>
        public void HappyAnim() {
            if(GoblinAnimID != null) StopCoroutine(GoblinAnimID);
            GoblinAnimID = StartCoroutine(CoHappyAnim());
        }

        IEnumerator CoHappyAnim() {
            while(true) {
                yield return BounceAnim();
            }
        }

        /// <summary>
        /// アニメーション停止
        /// </summary>
        public void StopGoblinAnim() {
            if(GoblinAnimID != null)
                StopCoroutine(GoblinAnimID);
            SpawnAnim();
        }

        // public void Update() {
        //     if (Input.GetKeyDown(KeyCode.A)) Character.Animator.SetTrigger("Attack");
        //     else if (Input.GetKeyDown(KeyCode.J)) Character.Animator.SetTrigger("Jab");
        //     else if (Input.GetKeyDown(KeyCode.P)) Character.Animator.SetTrigger("Push");
        //     else if (Input.GetKeyDown(KeyCode.H)) Character.Animator.SetTrigger("Hit");
        //     else if (Input.GetKeyDown(KeyCode.I)) { Character.SetState(AnimationState.Idle); _activityTime = 0; }
        //     else if (Input.GetKeyDown(KeyCode.R)) { Character.SetState(AnimationState.Ready); _activityTime = Time.time; }
        //     else if (Input.GetKeyDown(KeyCode.B)) Character.SetState(AnimationState.Blocking);
        //     else if (Input.GetKeyUp(KeyCode.B)) Character.SetState(AnimationState.Ready);
        //     else if (Input.GetKeyDown(KeyCode.D)) Character.SetState(AnimationState.Dead);

        //     // Builder characters only.
        //     if (Input.GetKeyDown(KeyCode.S)) Character.Animator.SetTrigger("Slash");
        //     else if (Input.GetKeyDown(KeyCode.O)) Character.Animator.SetTrigger("Shot");
        //     else if (Input.GetKeyDown(KeyCode.F)) Character.Animator.SetTrigger("Fire1H");
        //     else if (Input.GetKeyDown(KeyCode.E)) Character.Animator.SetTrigger("Fire2H");
        //     else if (Input.GetKeyDown(KeyCode.C)) Character.SetState(AnimationState.Climbing);
        //     else if (Input.GetKeyUp(KeyCode.C)) Character.SetState(AnimationState.Ready);
        //     else if (Input.GetKeyUp(KeyCode.L)) Character.Blink();

        //     // if (Controller.isGrounded)
        //     // {
        //     //     if (Input.GetKeyDown(KeyCode.DownArrow))
        //     //     {
        //     //         GetDown();
        //     //     }
        //     //     else if (Input.GetKeyUp(KeyCode.DownArrow))
        //     //     {
        //     //         GetUp();
        //     //     }
        //     // }

        //     // if (Input.GetKey(KeyCode.LeftArrow))
        //     // {
        //     //     _inputX = -1;
        //     // }
        //     // else if (Input.GetKey(KeyCode.RightArrow))
        //     // {
        //     //     _inputX = 1;
        //     // }
            
        //     // if (Input.GetKeyDown(KeyCode.UpArrow))
        //     // {
        //     //     _inputY = 1;
                
        //     //     if (Controller.isGrounded)
        //     //     {
        //     //         JumpDust.Play(true);
        //     //     }
        //     // }
        // }

        // public void FixedUpdate()
        // {
        //     Move();
        // }

        private void Move()
        {
            if (Time.frameCount <= 1)
            {
                Controller.Move(new Vector3(0, Gravity) * Time.fixedDeltaTime);
                return;
            }

            var state = Character.GetState();

            if (state == AnimationState.Dead)
            {
                if (_inputX == 0) return;

                Character.SetState(AnimationState.Running);
            }

            if (_inputX != 0)
            {
                Turn(_inputX);
            }

            if (Controller.isGrounded)
            {
                if (state == AnimationState.Jumping)
                {
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        GetDown();
                    }
                    else
                    {
                        Character.Animator.SetTrigger("Landed");
                        Character.SetState(AnimationState.Ready);
                        JumpDust.Play(true);
                    }
                }

                _motion = state == AnimationState.Crawling
                    ? new Vector3(CrawlSpeed * _inputX, 0)
                    : new Vector3(RunSpeed * _inputX, JumpSpeed * _inputY);

                if (_inputX != 0 || _inputY != 0)
                {
                    if (_inputY > 0)
                    {
                        Character.SetState(AnimationState.Jumping);
                    }
                    else
                    {
                        switch (state)
                        {
                            case AnimationState.Idle:
                            case AnimationState.Ready:
                                Character.SetState(AnimationState.Running);
                                break;
                        }
                    }
                }
                else
                {
                    switch (state)
                    {
                        case AnimationState.Crawling:
                        case AnimationState.Climbing:
                        case AnimationState.Blocking:
                            break;
                        default:
                            var targetState = Time.time - _activityTime > 5 ? AnimationState.Idle : AnimationState.Ready;

                            if (state != targetState)
                            {
                                Character.SetState(targetState);
                            }

                            break;
                    }
                }
            }
            else
            {
                _motion = new Vector3(RunSpeed * _inputX, _motion.y);
                Character.SetState(AnimationState.Jumping);
            }

            _motion.y += Gravity;

            Controller.Move(_motion * Time.fixedDeltaTime);

            Character.Animator.SetBool("Grounded", Controller.isGrounded);
            Character.Animator.SetBool("Moving", Controller.isGrounded && _inputX != 0);
            Character.Animator.SetBool("Falling", !Controller.isGrounded && Controller.velocity.y < 0);

            if (_inputX != 0 && _inputY != 0 || Character.Animator.GetBool("Action"))
            {
                _activityTime = Time.time;
            }

            _inputX = _inputY = 0;

            if (Controller.isGrounded && !Mathf.Approximately(Controller.velocity.x, 0))
            {
                var velocity = MoveDust.velocityOverLifetime;

                velocity.xMultiplier = 0.2f * -Mathf.Sign(Controller.velocity.x);

                if (!MoveDust.isPlaying)
                {
                    MoveDust.Play();
                }
            }
            else
            {
                MoveDust.Stop();
            }
        }

        private void Turn(int direction)
        {
            var scale = Character.transform.localScale;

            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);

            Character.transform.localScale = scale;
        }

        private void GetDown()
        {
            Character.Animator.SetTrigger("GetDown");
        }

        private void GetUp()
        {
            Character.Animator.SetTrigger("GetUp");
        }
    }
}