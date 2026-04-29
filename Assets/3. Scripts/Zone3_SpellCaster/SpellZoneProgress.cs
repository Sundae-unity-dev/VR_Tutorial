using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VRTutorial
{
    /// <summary>
    /// Zone3 타겟 수/격파 수를 직접 관리.
    /// SpellTarget이 Register/Unregister로 등록해 static 필드 없이 동작.
    /// </summary>
    public class SpellZoneProgress : MonoBehaviour
    {
        [SerializeField] TMP_Text progressText;
        [SerializeField] TutorialZoneManager zoneManager;

        readonly HashSet<SpellTarget> activeTargets = new HashSet<SpellTarget>();
        int destroyedCount = 0;
        int totalCount = 0;

        public void RegisterTarget(SpellTarget target)
        {
            if (activeTargets.Add(target))
                totalCount++;
            UpdateUI();
        }

        // OnDestroy 시 호출되므로 이미 격파된 타겟 Unregister와 구분
        public void UnregisterTarget(SpellTarget target)
        {
            activeTargets.Remove(target);
        }

        public void OnTargetDestroyed()
        {
            destroyedCount++;
            UpdateUI();

            if (destroyedCount >= totalCount)
                zoneManager?.CompleteCurrentZone();
        }

        void UpdateUI()
        {
            if (progressText)
                progressText.text = $"{destroyedCount} / {totalCount}";
        }
    }
}
