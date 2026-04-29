using UnityEngine;

namespace VRTutorial
{
    /// <summary>
    /// 순서가 있는 텔레포트 발판. 번호 순서대로 밟으면 Zone1 완료.
    /// </summary>
    public class TeleportPad : MonoBehaviour
    {
        [SerializeField] int padIndex;
        [SerializeField] Renderer padRenderer;
        [SerializeField] Color activeColor = Color.cyan;
        [SerializeField] Color completedColor = Color.green;

        static int nextRequired = 0;
        static int totalPads = 0;

        TeleportZoneProgress zoneProgress;

        void Awake()
        {
            totalPads++;
            zoneProgress = FindObjectOfType<TeleportZoneProgress>();
        }

        void OnDestroy() => totalPads--;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (padIndex != nextRequired) return;

            SetColor(completedColor);
            nextRequired++;
            zoneProgress?.OnPadReached(nextRequired, totalPads);
        }

        public void SetActive(bool active)
        {
            SetColor(active ? activeColor : Color.white);
        }

        void SetColor(Color c)
        {
            if (padRenderer) padRenderer.material.color = c;
        }
    }
}
