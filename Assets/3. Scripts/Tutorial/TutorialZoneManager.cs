using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace VRTutorial
{
    public class TutorialZoneManager : MonoBehaviour
    {
        [System.Serializable]
        public class Zone
        {
            public string zoneName;
            public GameObject zoneRoot;
            public TMP_Text titleText;
            public TMP_Text descriptionText;
            [TextArea] public string description;
            public UnityEvent onZoneComplete;
        }

        [SerializeField] Zone[] zones;
        [SerializeField] GameObject completionUI;

        int currentZone = -1;

        void Start() => ActivateZone(0);

        public void ActivateZone(int index)
        {
            if (index < 0 || index >= zones.Length) return;

            if (currentZone >= 0)
                zones[currentZone].zoneRoot?.SetActive(false);

            currentZone = index;
            var zone = zones[currentZone];
            zone.zoneRoot?.SetActive(true);

            if (zone.titleText) zone.titleText.text = zone.zoneName;
            if (zone.descriptionText) zone.descriptionText.text = zone.description;
        }

        public void CompleteCurrentZone()
        {
            zones[currentZone].onZoneComplete?.Invoke();

            // 씬 단위 전환이 설정된 경우 SceneTransitionManager로 위임
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.GoToNextZone();
                return;
            }

            int next = currentZone + 1;
            if (next < zones.Length)
                ActivateZone(next);
            else
                completionUI?.SetActive(true);
        }
    }
}
