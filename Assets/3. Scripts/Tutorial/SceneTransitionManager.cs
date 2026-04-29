using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRTutorial
{
    /// <summary>
    /// Zone 씬 간 전환을 담당.
    /// XR Origin(이 컴포넌트가 붙은 GameObject)과 TutorialSession을 DontDestroyOnLoad로 유지한다.
    /// Zone1_Locomotion 씬의 XR Origin에 붙여서 사용.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [Header("Scenes")]
        [SerializeField] string[] zoneSceneNames = {
            "Zone1_Locomotion",
            "Zone2_HandInteraction",
            "Zone3_SpellCaster"
        };

        [Header("Fade")]
        [SerializeField] CanvasGroup fadeCanvas;
        [SerializeField] float fadeDuration = 0.8f;

        public int CurrentZoneIndex { get; private set; } = 0;
        public string CurrentZoneName => zoneSceneNames[CurrentZoneIndex];

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // TutorialSession이 없으면 함께 생성
            if (TutorialSession.Instance == null)
                gameObject.AddComponent<TutorialSession>();
        }

        public void GoToNextZone()
        {
            int next = CurrentZoneIndex + 1;
            if (next < zoneSceneNames.Length)
                StartCoroutine(LoadZone(next));
            else
                StartCoroutine(ShowCompletion());
        }

        public void GoToZone(int index)
        {
            if (index >= 0 && index < zoneSceneNames.Length)
                StartCoroutine(LoadZone(index));
        }

        IEnumerator LoadZone(int index)
        {
            TutorialSession.Instance?.CompleteZone(CurrentZoneIndex);

            yield return StartCoroutine(Fade(1f));
            CurrentZoneIndex = index;
            yield return SceneManager.LoadSceneAsync(zoneSceneNames[index]);
            yield return StartCoroutine(Fade(0f));
        }

        IEnumerator ShowCompletion()
        {
            TutorialSession.Instance?.CompleteZone(CurrentZoneIndex);
            yield return StartCoroutine(Fade(1f));
            // 모든 존 완료 — 필요 시 결과 씬 추가 가능
            Debug.Log($"[Tutorial] All zones complete! Total score: {TutorialSession.Instance?.TotalScore}");
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
