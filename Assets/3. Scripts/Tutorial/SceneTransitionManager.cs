using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRTutorial
{
    /// <summary>
    /// Zone 씬 간 전환을 담당.
    /// Player 계층 어디에 붙여도 동작하도록 root GO를 DontDestroyOnLoad 처리한다.
    /// TutorialSession은 별도 root GO에 자동 생성한다.
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

            // Player 계층 전체(root GO)를 유지 — 어디에 붙어도 동작
            DontDestroyOnLoad(transform.root.gameObject);

            // TutorialSession은 독립 root GO로 생성해야 DontDestroyOnLoad 가능
            if (TutorialSession.Instance == null)
            {
                var sessionGO = new GameObject("[TutorialSession]");
                DontDestroyOnLoad(sessionGO);
                sessionGO.AddComponent<TutorialSession>();
            }
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
