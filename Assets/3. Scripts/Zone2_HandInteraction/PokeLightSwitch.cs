using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRTutorial
{
    /// <summary>
    /// 찌르기(Poke) 인터랙션으로 켜고 끄는 스위치.
    /// XRI PokeInteractable과 함께 사용. 컨트롤러 인터랙터에 햅틱 피드백 전송.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
    public class PokeLightSwitch : MonoBehaviour
    {
        [SerializeField] Light targetLight;
        [SerializeField] Renderer switchRenderer;
        [SerializeField] Color onColor = Color.yellow;
        [SerializeField] Color offColor = Color.gray;
        [SerializeField] AudioSource clickSound;
        [SerializeField] float hapticAmplitude = 0.5f;
        [SerializeField] float hapticDuration = 0.08f;
        public UnityEvent onToggle;

        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        static readonly int ColorId = Shader.PropertyToID("_Color");
        MaterialPropertyBlock mpb;

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;
        bool isOn = false;

        void Awake()
        {
            mpb = new MaterialPropertyBlock();
            interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        }

        void OnEnable()
        {
            interactable.selectEntered.AddListener(OnPoked);
        }

        void OnDisable()
        {
            interactable.selectEntered.RemoveListener(OnPoked);
        }

        void OnPoked(SelectEnterEventArgs args)
        {
            SendHaptic(args.interactorObject);
            Toggle();
        }

        // Inspector에서 직접 연결하는 경우를 위해 public 유지
        public void Toggle()
        {
            isOn = !isOn;
            if (targetLight) targetLight.enabled = isOn;
            SetColor(isOn ? onColor : offColor);
            clickSound?.Play();
            onToggle?.Invoke();
        }

        void SendHaptic(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
        {
            if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
                controllerInteractor.SendHapticImpulse(hapticAmplitude, hapticDuration);
        }

        void SetColor(Color c)
        {
            if (switchRenderer == null) return;
            switchRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(BaseColorId, c);
            mpb.SetColor(ColorId, c);
            switchRenderer.SetPropertyBlock(mpb);
        }
    }
}
