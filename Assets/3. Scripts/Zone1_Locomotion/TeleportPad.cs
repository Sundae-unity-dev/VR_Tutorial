using UnityEngine;

namespace VRTutorial
{
    /// <summary>
    /// 순서가 있는 텔레포트 발판. 번호 순서대로 밟으면 Zone1 완료.
    /// 카운터는 TeleportZoneProgress가 관리 (static 필드 사용 안함).
    /// </summary>
    public class TeleportPad : MonoBehaviour
    {
        [SerializeField] int padIndex;
        [SerializeField] Renderer padRenderer;
        [SerializeField] Color activeColor = Color.cyan;
        [SerializeField] Color completedColor = Color.green;

        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");
        MaterialPropertyBlock mpb;

        TeleportZoneProgress zoneProgress;

        void Awake()
        {
            mpb = new MaterialPropertyBlock();
            zoneProgress = FindObjectOfType<TeleportZoneProgress>();
            zoneProgress?.RegisterPad(this);
            SetColor(activeColor);
        }

        void OnDestroy()
        {
            zoneProgress?.UnregisterPad(this);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            zoneProgress?.OnPadReached(padIndex, this);
        }

        public void MarkCompleted() => SetColor(completedColor);

        void SetColor(Color c)
        {
            if (padRenderer == null) return;
            padRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(BaseColorId, c);
            mpb.SetColor(ColorId, c);
            padRenderer.SetPropertyBlock(mpb);
        }
    }
}
