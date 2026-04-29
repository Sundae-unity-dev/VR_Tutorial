using UnityEngine;
using TMPro;

namespace VRTutorial
{
    /// <summary>
    /// 마법에 맞으면 터지는 타겟. 일정 수 격파 시 Zone3 완료.
    /// </summary>
    public class SpellTarget : MonoBehaviour
    {
        [SerializeField] int hitsRequired = 1;
        [SerializeField] GameObject destroyEffect;
        [SerializeField] AudioClip popSound;

        static int totalDestroyed = 0;
        static int totalTargets = 0;

        SpellZoneProgress zoneProgress;
        int hitsReceived = 0;

        void Awake()
        {
            totalTargets++;
            zoneProgress = FindObjectOfType<SpellZoneProgress>();
        }

        void OnDestroy() => totalTargets--;

        public void OnHit()
        {
            hitsReceived++;
            if (hitsReceived < hitsRequired) return;

            totalDestroyed++;
            TutorialSession.Instance?.AddScore(2, 20);
            if (destroyEffect) Instantiate(destroyEffect, transform.position, Quaternion.identity);
            if (popSound) AudioSource.PlayClipAtPoint(popSound, transform.position);

            zoneProgress?.OnTargetDestroyed(totalDestroyed, totalTargets + totalDestroyed);
            Destroy(gameObject);
        }
    }
}
