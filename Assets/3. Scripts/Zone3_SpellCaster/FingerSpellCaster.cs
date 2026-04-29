using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

namespace VRTutorial
{
    /// <summary>
    /// 검지를 펴고 나머지 손가락을 접으면 마법 발사.
    /// XR Hands 서브시스템에서 직접 조인트 데이터를 읽어 포즈를 판별.
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
        [SerializeField] float pointingDotThreshold = 0.7f;   // 검지 펴짐 기준
        [SerializeField] float curledDotThreshold = -0.1f;    // 나머지 손가락 접힘 기준

        XRHandSubsystem handSubsystem;
        float lastFireTime = -99f;
        bool isCharging = false;
        float chargeStartTime;

        void OnEnable() => StartCoroutine(InitSubsystem());

        IEnumerator InitSubsystem()
        {
            yield return new WaitUntil(() =>
                XRGeneralSettings.Instance != null &&
                XRGeneralSettings.Instance.Manager != null);

            handSubsystem = XRGeneralSettings.Instance.Manager
                .activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
        }

        void Update()
        {
            if (handSubsystem == null) return;

            var hand = handedness == Handedness.Right
                ? handSubsystem.rightHand
                : handSubsystem.leftHand;

            if (!hand.isTracked) return;

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
            // 검지 끝 방향이 손 위쪽과 일치하면 펴진 것으로 판단
            if (!TryGetJointForward(hand, XRHandJointID.IndexDistal, out Vector3 indexDir)) return false;
            if (!TryGetJointForward(hand, XRHandJointID.Palm, out Vector3 palmUp)) return false;

            bool indexPointing = Vector3.Dot(indexDir, palmUp) > pointingDotThreshold;

            // 중지·약지·소지는 접혀있어야 함
            bool middleCurled = IsFingerCurled(hand, XRHandJointID.MiddleDistal, palmUp);
            bool ringCurled   = IsFingerCurled(hand, XRHandJointID.RingDistal, palmUp);
            bool pinkyCurled  = IsFingerCurled(hand, XRHandJointID.LittleDistal, palmUp);

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
            var joint = hand.GetJoint(jointId);
            if (!joint.TryGetPose(out Pose pose)) return false;
            forward = pose.rotation * Vector3.forward;
            return true;
        }

        void TryFire(XRHand hand)
        {
            if (Time.time - lastFireTime < fireCooldown) return;
            if (spellProjectilePrefab == null) return;

            // 검지 끝에서 발사 방향 계산
            Transform origin = spellSpawnPoint ? spellSpawnPoint : transform;
            Vector3 dir = origin.forward;

            if (hand.GetJoint(XRHandJointID.IndexDistal).TryGetPose(out Pose indexPose))
            {
                dir = indexPose.rotation * Vector3.forward;
                origin.position = indexPose.position;
            }

            var proj = Instantiate(spellProjectilePrefab, origin.position, Quaternion.LookRotation(dir));
            if (proj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.useGravity = false;
                rb.linearVelocity = dir * projectileSpeed;
            }

            lastFireTime = Time.time;
        }
    }
}
