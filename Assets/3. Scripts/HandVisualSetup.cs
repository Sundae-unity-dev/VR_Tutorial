using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Left Controller / Right Controller 에 직접 붙이세요.
/// XR Hands 서브시스템 없이 Device Simulator에서도 손이 보입니다.
/// trigger → 검지, grip → 나머지 손가락 구동.
/// </summary>
public class HandVisualSetup : MonoBehaviour
{
    [SerializeField] GameObject handModelPrefab;
    [SerializeField] InputActionReference triggerAction;
    [SerializeField] InputActionReference gripAction;

    [Header("Finger Bones")]
    [SerializeField] Transform[] indexBones;
    [SerializeField] Transform[] middleBones;
    [SerializeField] Transform[] ringBones;
    [SerializeField] Transform[] pinkyBones;
    [SerializeField] Transform[] thumbBones;

    GameObject handInstance;

    void Start()
    {
        // 이 컨트롤러 아래에 있는 기존 메시 렌더러 숨김
        foreach (var r in GetComponentsInChildren<MeshRenderer>(true))
            r.enabled = false;
        foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>(true))
            r.enabled = false;

        if (handModelPrefab == null) return;

        handInstance = Instantiate(handModelPrefab, transform);
        handInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // 인스턴싱 후 bone 참조를 새 인스턴스 기준으로 자동 매핑
        AutoMapBones(handInstance.transform);
    }

    void Update()
    {
        if (handInstance == null) return;

        float trigger = triggerAction != null ? triggerAction.action.ReadValue<float>() : 0f;
        float grip    = gripAction    != null ? gripAction.action.ReadValue<float>()    : 0f;

        CurlFinger(indexBones,  trigger * 75f);
        CurlFinger(thumbBones,  trigger * 40f);
        CurlFinger(middleBones, grip * 80f);
        CurlFinger(ringBones,   grip * 80f);
        CurlFinger(pinkyBones,  grip * 80f);
    }

    void CurlFinger(Transform[] bones, float angle)
    {
        if (bones == null) return;
        foreach (var bone in bones)
            if (bone != null)
                bone.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    // LeftHand.fbx / RightHand.fbx 의 본 이름 패턴으로 자동 매핑
    void AutoMapBones(Transform root)
    {
        indexBones  = FindBones(root, "Index");
        middleBones = FindBones(root, "Middle");
        ringBones   = FindBones(root, "Ring");
        pinkyBones  = FindBones(root, "Little", "Pinky");
        thumbBones  = FindBones(root, "Thumb");
    }

    Transform[] FindBones(Transform root, params string[] keywords)
    {
        var result = new System.Collections.Generic.List<Transform>();
        CollectBones(root, keywords, result);
        result.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
        return result.ToArray();
    }

    void CollectBones(Transform t, string[] keywords, System.Collections.Generic.List<Transform> result)
    {
        foreach (string kw in keywords)
            if (t.name.Contains(kw)) { result.Add(t); break; }
        foreach (Transform child in t)
            CollectBones(child, keywords, result);
    }
}
