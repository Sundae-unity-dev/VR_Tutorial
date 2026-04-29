using UnityEngine;
using TMPro;

namespace VRTutorial
{
    /// <summary>
    /// 바구니 영역에 Grabbable 오브젝트가 들어오면 점수 집계.
    /// </summary>
    public class BasketScorer : MonoBehaviour
    {
        [SerializeField] int requiredScore = 3;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TutorialZoneManager zoneManager;
        [SerializeField] AudioSource successSound;

        int score = 0;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Grabbable")) return;

            score++;
            TutorialSession.Instance?.AddScore(1, 10);
            UpdateUI();
            successSound?.Play();

            if (score >= requiredScore)
                zoneManager?.CompleteCurrentZone();
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Grabbable")) return;
            score = Mathf.Max(0, score - 1);
            UpdateUI();
        }

        void UpdateUI()
        {
            if (scoreText) scoreText.text = $"{score} / {requiredScore}";
        }
    }
}
