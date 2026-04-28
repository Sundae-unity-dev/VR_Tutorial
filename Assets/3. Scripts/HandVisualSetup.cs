using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

/// <summary>
/// Play 시작 시 컨트롤러 모델을 숨기고 손 비주얼 프리팹을 Camera Offset 아래에 배치합니다.
/// XR Origin (XR Rig) 루트 오브젝트에 붙여주세요.
/// </summary>
public class HandVisualSetup : MonoBehaviour
{
    [SerializeField] GameObject leftHandPrefab;
    [SerializeField] GameObject rightHandPrefab;

    void Awake()
    {
        var cameraOffset = FindDeep(transform, "Camera Offset");
        if (cameraOffset == null)
        {
            Debug.LogError("[HandVisualSetup] Camera Offset을 찾을 수 없습니다.");
            return;
        }

        // 컨트롤러 모델 비활성화
        var leftCtrl  = FindDeep(transform, "Left Controller");
        var rightCtrl = FindDeep(transform, "Right Controller");
        if (leftCtrl)  leftCtrl.gameObject.SetActive(false);
        if (rightCtrl) rightCtrl.gameObject.SetActive(false);

        // 손 비주얼 인스턴싱
        GameObject leftInstance  = null;
        GameObject rightInstance = null;

        if (leftHandPrefab != null)
        {
            leftInstance = Instantiate(leftHandPrefab, cameraOffset);
            leftInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        if (rightHandPrefab != null)
        {
            rightInstance = Instantiate(rightHandPrefab, cameraOffset);
            rightInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        // XRInputModalityManager에 손 오브젝트 연결
        var modalityManager = GetComponentInChildren<XRInputModalityManager>(true);
        if (modalityManager != null)
        {
            if (leftInstance  != null) modalityManager.leftHand  = leftInstance;
            if (rightInstance != null) modalityManager.rightHand = rightInstance;
        }
    }

    static Transform FindDeep(Transform parent, string targetName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == targetName) return child;
            var found = FindDeep(child, targetName);
            if (found != null) return found;
        }
        return null;
    }
}
