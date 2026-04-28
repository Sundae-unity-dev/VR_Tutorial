using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SetupHandTracking
{
    const string LeftHandPrefabPath  = "Assets/Samples/XR Interaction Toolkit/3.5.0/Hands Interaction Demo/Prefabs/LeftHandQuestVisual.prefab";
    const string RightHandPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.5.0/Hands Interaction Demo/Prefabs/RightHandQuestVisual.prefab";

    [MenuItem("VR Tutorial/Setup Hand Tracking Visuals")]
    static void Setup()
    {
        var modalityManager = Object.FindFirstObjectByType<XRInputModalityManager>();
        if (modalityManager == null)
        {
            Debug.LogError("[SetupHandTracking] XRInputModalityManager를 씬에서 찾을 수 없습니다.");
            return;
        }

        var leftPrefab  = AssetDatabase.LoadAssetAtPath<GameObject>(LeftHandPrefabPath);
        var rightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RightHandPrefabPath);

        if (leftPrefab == null || rightPrefab == null)
        {
            Debug.LogError("[SetupHandTracking] 손 프리팹을 찾을 수 없습니다. Samples 경로를 확인하세요.");
            return;
        }

        // Camera Offset 탐색
        var cameraOffset = FindCameraOffset(modalityManager.transform);
        if (cameraOffset == null)
        {
            Debug.LogError("[SetupHandTracking] Camera Offset을 찾을 수 없습니다.");
            return;
        }

        // 기존 인스턴스 제거
        RemoveExisting(cameraOffset, "LeftHandQuestVisual");
        RemoveExisting(cameraOffset, "RightHandQuestVisual");

        // 프리팹 인스턴싱
        var leftInstance  = (GameObject)PrefabUtility.InstantiatePrefab(leftPrefab,  cameraOffset);
        var rightInstance = (GameObject)PrefabUtility.InstantiatePrefab(rightPrefab, cameraOffset);

        leftInstance.name  = "LeftHandQuestVisual";
        rightInstance.name = "RightHandQuestVisual";

        // 위치 초기화
        leftInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        rightInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // XRInputModalityManager에 연결
        Undo.RecordObject(modalityManager, "Setup Hand Tracking");
        modalityManager.leftHand  = leftInstance;
        modalityManager.rightHand = rightInstance;

        EditorUtility.SetDirty(modalityManager);
        EditorSceneManager.MarkSceneDirty(modalityManager.gameObject.scene);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("[SetupHandTracking] 손 트래킹 비주얼 설정 완료!");
        Selection.activeGameObject = modalityManager.gameObject;
    }

    static Transform FindCameraOffset(Transform root)
    {
        // XR Origin 루트에서 위로 올라가며 탐색
        var xrOriginRoot = root;
        while (xrOriginRoot.parent != null)
            xrOriginRoot = xrOriginRoot.parent;

        return FindChildByName(xrOriginRoot, "Camera Offset");
    }

    static Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var found = FindChildByName(child, name);
            if (found != null) return found;
        }
        return null;
    }

    static void RemoveExisting(Transform parent, string name)
    {
        var existing = parent.Find(name);
        if (existing != null)
            Undo.DestroyObjectImmediate(existing.gameObject);
    }
}
