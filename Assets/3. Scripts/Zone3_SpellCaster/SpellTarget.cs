using UnityEngine;

namespace VRTutorial
{
    /// <summary>
    /// 마법에 맞으면 터지는 타겟.
    /// 카운터는 SpellZoneProgress가 관리 (static 필드 사용 안함).
    /// </summary>
    public class SpellTarget : MonoBehaviour
    {
        [SerializeField] int hitsRequired = 1;
        [SerializeField] GameObject destroyEffect;
        [SerializeField] AudioClip popSound;

        SpellZoneProgress zoneProgress;
        int hitsReceived = 0;

        void Awake()
        {
            zoneProgress = FindObjectOfType<SpellZoneProgress>();
            zoneProgress?.RegisterTarget(this);
        }

        void OnDestroy()
        {
            zoneProgress?.UnregisterTarget(this);
        }

        public void OnHit()
        {
            hitsReceived++;
            if (hitsReceived < hitsRequired) return;

            TutorialSession.Instance?.AddScore(2, 20);

            if (destroyEffect)
                Instantiate(destroyEffect, transform.position, Quaternion.identity);

            AudioHelper.Play3D(popSound, transform.position);

            // OnDestroy에서 UnregisterTarget이 호출되므로 먼저 진행 알림
            zoneProgress?.OnTargetDestroyed();
            Destroy(gameObject);
        }
    }
}
