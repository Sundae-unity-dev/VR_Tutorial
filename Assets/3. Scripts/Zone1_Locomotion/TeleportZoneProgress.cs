using UnityEngine;
using TMPro;

namespace VRTutorial
{
    public class TeleportZoneProgress : MonoBehaviour
    {
        [SerializeField] TMP_Text progressText;
        [SerializeField] TutorialZoneManager zoneManager;

        public void OnPadReached(int reached, int total)
        {
            if (progressText)
                progressText.text = $"{reached} / {total}";

            if (reached >= total)
                zoneManager?.CompleteCurrentZone();
        }
    }
}
