using UnityEngine;
using UnityEngine.Events;

namespace VRTutorial
{
    /// <summary>
    /// 찌르기(Poke) 인터랙션으로 켜고 끄는 스위치.
    /// XRI PokeInteractable과 함께 사용.
    /// </summary>
    public class PokeLightSwitch : MonoBehaviour
    {
        [SerializeField] Light targetLight;
        [SerializeField] Renderer switchRenderer;
        [SerializeField] Color onColor = Color.yellow;
        [SerializeField] Color offColor = Color.gray;
        [SerializeField] AudioSource clickSound;
        public UnityEvent onToggle;

        bool isOn = false;

        public void Toggle()
        {
            isOn = !isOn;
            if (targetLight) targetLight.enabled = isOn;
            if (switchRenderer) switchRenderer.material.color = isOn ? onColor : offColor;
            clickSound?.Play();
            onToggle?.Invoke();
        }
    }
}
