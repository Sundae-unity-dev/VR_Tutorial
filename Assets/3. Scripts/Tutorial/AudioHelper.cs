using UnityEngine;

namespace VRTutorial
{
    /// <summary>
    /// AudioSource.PlayClipAtPoint 대체 유틸리티.
    /// SpatialBlend=1(완전 3D)로 임시 GameObject를 생성해 재생 후 자동 소멸.
    /// </summary>
    public static class AudioHelper
    {
        public static void Play3D(AudioClip clip, Vector3 position, float volume = 1f,
            float minDistance = 1f, float maxDistance = 15f)
        {
            if (clip == null) return;

            var go = new GameObject("[Audio_OneShot]");
            go.transform.position = position;

            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1f;
            source.volume = volume;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.Play();

            Object.Destroy(go, clip.length + 0.1f);
        }
    }
}
