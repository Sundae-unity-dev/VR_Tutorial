using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRTutorial.Editor
{
    /// <summary>
    /// Window > VR Tutorial > Draw Call Optimizer
    /// Static Batching 자동 설정 및 최적화 상태 리포트.
    /// </summary>
    public class DrawCallOptimizer : EditorWindow
    {
        // Static으로 표시할 태그 (씬 오브젝트 중 움직이지 않는 것)
        static readonly string[] StaticTags = { "Environment", "Ground", "Untagged" };

        // Dynamic(Static 제외)으로 유지할 컴포넌트 타입명
        static readonly string[] DynamicComponentNames =
        {
            "TeleportPad", "SpellTarget", "PokeLightSwitch", "BasketScorer",
            "XRGrabInteractable", "XRPokeInteractable", "TeleportationArea",
            "TeleportationAnchor", "Rigidbody"
        };

        Vector2 scroll;
        List<string> reportLines = new List<string>();

        [MenuItem("Window/VR Tutorial/Draw Call Optimizer")]
        static void Open() => GetWindow<DrawCallOptimizer>("Draw Call Optimizer");

        void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("VR Tutorial — Draw Call Optimizer", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Quest 3 목표: Draw Call 50~100 이하\n" +
                "Static Batching은 같은 Material을 쓰는 정적 오브젝트들을 GPU에서 하나로 묶습니다.",
                MessageType.Info);

            EditorGUILayout.Space(4);

            if (GUILayout.Button("📊  현재 상태 분석", GUILayout.Height(30)))
                Analyze();

            EditorGUILayout.Space(4);

            if (GUILayout.Button("⚡  환경 오브젝트 Static 자동 설정", GUILayout.Height(30)))
                ApplyStaticBatching();

            EditorGUILayout.Space(8);

            if (reportLines.Count > 0)
            {
                EditorGUILayout.LabelField("분석 결과", EditorStyles.boldLabel);
                scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
                foreach (var line in reportLines)
                    EditorGUILayout.LabelField(line);
                EditorGUILayout.EndScrollView();
            }
        }

        void Analyze()
        {
            reportLines.Clear();
            var allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);

            int staticCount = 0, dynamicCount = 0;
            var materialGroups = new Dictionary<Material, int>();

            foreach (var r in allRenderers)
            {
                bool isStatic = GameObjectUtility.GetStaticEditorFlags(r.gameObject)
                    .HasFlag(StaticEditorFlags.BatchingStatic);

                if (isStatic) staticCount++;
                else dynamicCount++;

                foreach (var mat in r.sharedMaterials)
                {
                    if (mat == null) continue;
                    materialGroups.TryGetValue(mat, out int count);
                    materialGroups[mat] = count + 1;
                }
            }

            // GPU Instancing 미설정 머티리얼
            var noInstancing = new List<string>();
            foreach (var kv in materialGroups)
            {
                if (kv.Value >= 2 && !kv.Key.enableInstancing)
                    noInstancing.Add($"  • {kv.Key.name} ({kv.Value}개 사용 중)");
            }

            reportLines.Add($"총 Renderer: {allRenderers.Length}개");
            reportLines.Add($"  Static(배칭 가능): {staticCount}개");
            reportLines.Add($"  Dynamic:          {dynamicCount}개");
            reportLines.Add("");
            reportLines.Add($"사용 중인 Material: {materialGroups.Count}개");

            if (noInstancing.Count > 0)
            {
                reportLines.Add("");
                reportLines.Add($"⚠  GPU Instancing 미설정 (2개 이상 사용 중):");
                reportLines.AddRange(noInstancing);
            }
            else
            {
                reportLines.Add("✓  GPU Instancing 대상 Material 없음 (모두 설정됨)");
            }

            reportLines.Add("");
            reportLines.Add("※ 실제 Draw Call은 Play 모드 > Window > Analysis > Frame Debugger에서 확인");
        }

        void ApplyStaticBatching()
        {
            reportLines.Clear();
            int applied = 0, skipped = 0;

            var allGOs = FindObjectsByType<GameObject>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (var go in allGOs)
            {
                // 인터랙션 컴포넌트가 있으면 Dynamic 유지
                if (IsDynamic(go))
                {
                    skipped++;
                    continue;
                }

                // Renderer 없으면 건너뜀 (빈 GO, 매니저 등)
                if (!go.TryGetComponent<Renderer>(out _))
                    continue;

                var flags = GameObjectUtility.GetStaticEditorFlags(go);
                if (!flags.HasFlag(StaticEditorFlags.BatchingStatic))
                {
                    GameObjectUtility.SetStaticEditorFlags(go,
                        flags | StaticEditorFlags.BatchingStatic
                               | StaticEditorFlags.OccluderStatic
                               | StaticEditorFlags.OccludeeStatic);
                    applied++;
                }
            }

            EditorApplication.MarkSceneDirty();

            reportLines.Add($"✓  Static 설정 완료: {applied}개");
            reportLines.Add($"   Dynamic 유지:    {skipped}개 (인터랙션 컴포넌트 있음)");
            reportLines.Add("");
            reportLines.Add("씬을 저장(Ctrl+S)하고 Window > Rendering > Lighting > Generate Lighting을 실행하세요.");
        }

        static bool IsDynamic(GameObject go)
        {
            foreach (var name in DynamicComponentNames)
                if (go.GetComponent(name) != null) return true;

            // Rigidbody가 있으면 물리 오브젝트 → Dynamic
            if (go.TryGetComponent<Rigidbody>(out _)) return true;
            if (go.TryGetComponent<Animator>(out _)) return true;

            return false;
        }
    }
}
