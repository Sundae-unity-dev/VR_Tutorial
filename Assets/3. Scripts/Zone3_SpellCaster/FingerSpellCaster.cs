using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

namespace VRTutorial
{
    /// <summary>
    /// 검지를 펴고 나머지 손가락을 접으면 마법 발사.
    /// </summary>
    public class FingerSpellCaster : MonoBehaviour
    {
        [Header("Hand")]
        [SerializeField] Handedness handedness = Handedness.Right;

        [Header("Spell")]
        [SerializeField] GameObject spellProjectilePrefab;
        [SerializeField] Transform spellSpawnPoint;
        [SerializeField] float projectileSpeed = 8f;
        [SerializeField] float fireCooldown = 0.6f;

        [Header("Charge VFX")]
        [SerializeField] ParticleSystem chargeEffect;
        [SerializeField] float chargeTime = 0.4f;

        [Header("Pose Thresholds")]
        [SerializeField] float pointingDotThreshold = 0.7f;
        [SerializeField] float curledDotThreshold = -0.1f;

        XRHandSubsystem handSubsystem;
        float lastFireTime = -99f;
        bool isCharging = false;
        float chargeStartTime;

        void OnEnable()
        {
            // SubsystemManager 방식이 XRGeneralSettings보다 안정적
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            handSubsystem = subsystems.Count > 0 ? subsystems[0] : null;

            // 아직 초기화 안된 경우 대기
            if (handSubsystem == null)
                XRGeneralSettings.Instance?.Manager?.activeLoader
                    ?.GetLoadedSubsystem<XRHandSubsystem>();
        }

        void OnDisable()
        {
            handSubsystem = null;
            isCharging = false;
            chargeEffect?.Stop();
        }

        void Update()
        {
            if (handSubsystem == null) return;

            var hand = handedness == Handedness.Right
                ? handSubsystem.rightHand
                : handSubsystem.leftHand;

            if (!hand.isTracked)
            {
                if (isCharging) { isCharging = false; chargeEffect?.Stop(); }
                return;
            }

            bool posing = IsPointingPose(hand);

            if (posing && !isCharging)
            {
                isCharging = true;
                chargeStartTime = Time.time;
                chargeEffect?.Play();
            }
            else if (!posing && isCharging)
            {
                isCharging = false;
                chargeEffect?.Stop();
            }

            if (isCharging && Time.time - chargeStartTime >= chargeTime)
            {
                isCharging = false;
                chargeEffect?.Stop();
                TryFire(hand);
            }
        }

        bool IsPointingPose(XRHand hand)
        {
            if (!TryGetJointForward(hand, XRHandJointID.IndexDistal, out Vector3 indexDir)) return false;
            if (!TryGetJointForward(hand, XRHandJointID.Palm, out Vector3 palmUp)) return false;

            bool indexPointing = Vector3.Dot(indexDir, palmUp) > pointingDotThreshold;
            bool middleCurled  = IsFingerCurled(hand, XRHandJointID.MiddleDistal, palmUp);
            bool ringCurled    = IsFingerCurled(hand, XRHandJointID.RingDistal, palmUp);
            bool pinkyCurled   = IsFingerCurled(hand, XRHandJointID.LittleDistal, palmUp);

            return indexPointing && middleCurled && ringCurled && pinkyCurled;
        }

        bool IsFingerCurled(XRHand hand, XRHandJointID tipJoint, Vector3 palmUp)
        {
            if (!TryGetJointForward(hand, tipJoint, out Vector3 dir)) return false;
            return Vector3.Dot(dir, palmUp) < curledDotThreshold;
        }

        bool TryGetJointForward(XRHand hand, XRHandJointID jointId, out Vector3 forward)
        {
            forward = Vector3.forward;
            if (!hand.GetJoint(jointId).TryGetPose(out Pose pose)) return false;
            forward = pose.rotation * Vector3.forward;
            return true;
        }

        void TryFire(XRHand hand)
        {
            if (Time.time - lastFireTime < fireCooldown) return;
            if (spellProjectilePrefab == null) return;

            // spawnPoint Transform을 직접 수정하지 않고 로컬 변수로 처리
            Vector3 spawnPos = spellSpawnPoint ? spellSpawnPoint.position : transform.position;
            Vector3 dir = spellSpawnPoint ? spellSpawnPoint.forward : transform.forward;

            if (hand.GetJoint(XRHandJointID.IndexDistal).TryGetPose(out Pose indexPose))
            {
                spawnPos = indexPose.position;
                dir = indexPose.rotation * Vector3.forward;
            }

            var proj = Instantiate(spellProjectilePrefab, spawnPos, Quaternion.LookRotation(dir));
            if (proj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.useGravity = false;
                rb.velocity = dir * projectileSpeed;
            }

            lastFireTime = Time.time;
        }
    }
}
