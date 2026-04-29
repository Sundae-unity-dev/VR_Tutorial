using UnityEngine;

namespace VRTutorial
{
    /// <summary>
    /// 발사된 마법 프로젝타일. 타겟에 맞으면 이펙트 재생 후 소멸.
    /// </summary>
    public class SpellProjectile : MonoBehaviour
    {
        [SerializeField] float lifetime = 4f;
        [SerializeField] GameObject hitEffectPrefab;
        [SerializeField] AudioClip hitSound;

        void Start() => Destroy(gameObject, lifetime);

        void OnCollisionEnter(Collision col)
        {
            if (hitEffectPrefab)
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            AudioHelper.Play3D(hitSound, transform.position);

            col.gameObject.GetComponent<SpellTarget>()?.OnHit();
            Destroy(gameObject);
        }
    }
}
