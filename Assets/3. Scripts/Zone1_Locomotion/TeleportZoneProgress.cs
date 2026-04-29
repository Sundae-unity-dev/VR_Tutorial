using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VRTutorial
{
    /// <summary>
    /// Zone1 발판 순서 진행을 관리.
    /// TeleportPad가 Register/Unregister로 등록해 static 필드 없이 동작.
    /// </summary>
    public class TeleportZoneProgress : MonoBehaviour
    {
        [SerializeField] TMP_Text progressText;
        [SerializeField] TutorialZoneManager zoneManager;

        readonly List<TeleportPad> pads = new List<TeleportPad>();
        int nextRequired = 0;
        int reachedCount = 0;

        public void RegisterPad(TeleportPad pad)
        {
            if (!pads.Contains(pad))
                pads.Add(pad);
            UpdateUI();
        }

        public void UnregisterPad(TeleportPad pad)
        {
            pads.Remove(pad);
        }

        public void OnPadReached(int padIndex, TeleportPad pad)
        {
            if (padIndex != nextRequired) return;

            pad.MarkCompleted();
            nextRequired++;
            reachedCount++;
            UpdateUI();

            if (reachedCount >= pads.Count)
                zoneManager?.CompleteCurrentZone();
        }

        void UpdateUI()
        {
            if (progressText)
                progressText.text = $"{reachedCount} / {pads.Count}";
        }
    }
}
