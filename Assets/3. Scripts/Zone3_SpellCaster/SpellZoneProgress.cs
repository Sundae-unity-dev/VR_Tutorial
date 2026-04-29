using UnityEngine;
using TMPro;

namespace VRTutorial
{
    public class SpellZoneProgress : MonoBehaviour
    {
        [SerializeField] TMP_Text progressText;
        [SerializeField] TutorialZoneManager zoneManager;

        public void OnTargetDestroyed(int destroyed, int total)
        {
            if (progressText)
                progressText.text = $"{destroyed} / {total}";

            if (destroyed >= total)
                zoneManager?.CompleteCurrentZone();
        }
    }
}
