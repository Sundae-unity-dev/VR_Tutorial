using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace VRTutorial
{
    /// <summary>
    /// Zone 씬 간 전환을 담당. XR Origin은 DontDestroyOnLoad로 유지.
    /// Zone1 씬에서 이 컴포넌트를 XR Origin에 붙여두면 된다.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [Header("Scenes (Build Settings 순서와 일치)")]
        [SerializeField] string[] zoneSceneNames = {
            "Zone1_Locomotion",
            "Zone2_HandInteraction",
            "Zone3_SpellCaster"
        };

        [Header("Fade")]
        [SerializeField] CanvasGroup fadeCanvas;
        [SerializeField] float fadeDuration = 0.8f;

        int currentZoneIndex = 0;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void GoToNextZone()
        {
            int next = currentZoneIndex + 1;
            if (next < zoneSceneNames.Length)
                StartCoroutine(LoadZone(next));
        }

        public void GoToZone(int index)
        {
            if (index >= 0 && index < zoneSceneNames.Length)
                StartCoroutine(LoadZone(index));
        }

        IEnumerator LoadZone(int index)
        {
            yield return StartCoroutine(Fade(1f));

            currentZoneIndex = index;
            yield return SceneManager.LoadSceneAsync(zoneSceneNames[index]);

            yield return StartCoroutine(Fade(0f));
        }

        IEnumerator Fade(float targetAlpha)
        {
            if (fadeCanvas == null) yield break;
            fadeCanvas.gameObject.SetActive(true);
            float start = fadeCanvas.alpha;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(start, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }
            fadeCanvas.alpha = targetAlpha;
            if (targetAlpha == 0f) fadeCanvas.gameObject.SetActive(false);
        }
    }
}
