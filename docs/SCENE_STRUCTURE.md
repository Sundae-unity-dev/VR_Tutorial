# 씬 구조 설계

## 1. 설계 원칙
1. **본 게임과 공유 가능한 자산은 prefab 으로 분리** — 컨트롤 매핑이 한 곳에서만 변경되도록 한다.
2. **튜토리얼은 단일 씬**으로 구성하고, 미션 단계는 ScriptableObject + Trigger 로 순회한다.
   씬을 단계마다 나누면 로딩 끊김과 상태 전달이 번거로워진다.
3. **씬 전환은 GameManager 한 곳에서만** 처리한다.
4. **저장은 PlayerPrefs 로 최소화**한다 (마지막 완료 단계 인덱스만 저장).

## 2. 씬 흐름
```
00_Bootstrap → 01_MainMenu → 02_Tutorial → 03_GameEntry
        ↑                          │
        └──── (재시작 선택 시) ─────┘
```

| 씬 | 역할 | 주요 오브젝트 |
|---|---|---|
| `00_Bootstrap` | 매니저·세팅 로드 후 자동으로 다음 씬으로 전환 | `GameManager`, `AudioManager`, `XRSettings` |
| `01_MainMenu` | "튜토리얼 시작" / "본 게임 바로 시작" 분기 | World Canvas, XR_Rig (대기 위치) |
| `02_Tutorial` | 본 튜토리얼. 미션 8단계가 순차 활성화 | XR_Rig, MissionController, MissionTrigger 8개 |
| `03_GameEntry` | 본 게임 진입 placeholder. 실제 본 게임 씬으로 교체될 슬롯 | (외부 프로젝트 연결 시 대체) |

## 3. 폴더 구조
```
Assets/
├─ _Project/                       # 우리 프로젝트 자산은 모두 이 아래
│  ├─ Scenes/
│  │  ├─ 00_Bootstrap.unity
│  │  ├─ 01_MainMenu.unity
│  │  ├─ 02_Tutorial.unity
│  │  └─ 03_GameEntry.unity
│  ├─ Prefabs/
│  │  ├─ XR/                       # ← 본 게임과 공유 후보
│  │  │  ├─ XR_Rig.prefab
│  │  │  ├─ Hand_Left.prefab
│  │  │  └─ Hand_Right.prefab
│  │  ├─ Tutorial/
│  │  │  ├─ MissionTrigger.prefab
│  │  │  ├─ GuidePanel.prefab
│  │  │  ├─ ControllerHint.prefab
│  │  │  └─ TeleportTarget.prefab
│  │  └─ Interactables/
│  │     ├─ GrabCube.prefab
│  │     ├─ UIButton_World.prefab
│  │     └─ SocketSlot.prefab
│  ├─ ScriptableObjects/
│  │  └─ Missions/                  # 단계별 SO 인스턴스
│  │     ├─ Mission_00_LookAround.asset
│  │     ├─ Mission_01_Controllers.asset
│  │     └─ ... (총 8개)
│  ├─ Scripts/
│  │  ├─ Core/
│  │  │  ├─ GameManager.cs          # 씬 전환 단일 진입점
│  │  │  ├─ AudioManager.cs
│  │  │  └─ TutorialProgress.cs     # PlayerPrefs 저장/로드
│  │  ├─ Tutorial/
│  │  │  ├─ MissionStep.cs          # ScriptableObject 베이스
│  │  │  ├─ MissionController.cs    # 단계 순회·이벤트 발행
│  │  │  ├─ MissionTrigger.cs
│  │  │  ├─ GuidePanelView.cs
│  │  │  └─ steps/
│  │  │     ├─ LookAroundStep.cs
│  │  │     ├─ ControllerCheckStep.cs
│  │  │     ├─ TriggerStep.cs
│  │  │     ├─ DirectGrabStep.cs
│  │  │     ├─ RayGrabStep.cs
│  │  │     ├─ TeleportStep.cs
│  │  │     ├─ UIClickStep.cs
│  │  │     └─ IntegratedStep.cs
│  │  └─ Input/
│  │     └─ XRInputActions.inputactions
│  ├─ Audio/
│  │  └─ Narration/
│  │     ├─ ko/
│  │     └─ (en/) (차후)
│  └─ Materials, Textures, Models/
└─ Plugins/                          # XRI, OpenXR 등 패키지
```

## 4. 핵심 설계 포인트

### 4.1 본 게임과 자산 공유
`XR_Rig.prefab` 와 `XRInputActions.inputactions` 는 본 게임 프로젝트와 동일해야 한다.
세 가지 옵션 중 택 1:

| 방식 | 장점 | 단점 |
|---|---|---|
| (A) **Git submodule**: 두 프로젝트가 공통 repo 참조 | 단일 진실의 원천 | 셋업 복잡, Unity가 submodule 처리 까다로움 |
| (B) **Unity Package (UPM Git URL)**: 공유 자산을 별도 패키지로 | Unity 친화적, 의존성 관리 명확 | 초기 패키지화 작업 필요 |
| (C) **수동 동기화**: 변경 시 양쪽에 복사 | 셋업 즉시 가능 | 휴먼 에러 발생 가능 |

**권장: (B) UPM Package.** 본 게임과의 연동이 확정되면 `com.sundae.vrcore` 형태로 분리.

### 4.2 미션을 ScriptableObject 로 정의하는 이유
- 단계 추가/순서 변경이 코드 수정 없이 인스펙터에서 가능
- 비개발자(기획자)가 텍스트, 음성 클립, 성공 조건 파라미터를 직접 편집 가능
- 같은 단계를 여러 조합으로 재사용 가능 (예: 본 게임 안에서 부분 재교육)

### 4.3 미션 진행 흐름
```
MissionController
  ├─ List<MissionStep> steps  (Inspector 에서 순서 설정)
  ├─ int currentIndex
  └─ Update():
       steps[currentIndex].Tick()
       if (steps[currentIndex].IsComplete)
           steps[currentIndex].OnExit();
           currentIndex++;
           if (currentIndex < steps.Count) steps[currentIndex].OnEnter();
           else FinishTutorial();
```
각 `MissionStep` 은 자신의 `MissionTrigger`/`Interactable` 만 활성화한다.
다른 단계의 오브젝트는 비활성 상태로 두어 사용자가 순서를 건너뛰지 못하게 한다.

### 4.4 씬 전환 단일화
모든 씬 전환은 `GameManager.LoadScene(SceneId)` 만 사용한다.
직접 `SceneManager.LoadScene` 호출 금지 — 페이드, 진행상황 저장, 오디오 정리 누락 방지.

## 5. 리스크 & 대응
| 리스크 | 대응 |
|---|---|
| Quest 환경에서 90 FPS 미달 | 라이트맵 베이크, GPU Instancing, 라이트 1개 + 베이크된 환경 |
| 미션 성공 판정 미스 (사용자가 정상 행동했는데 판정 누락) | 5초 미응답 시 힌트 강조 + 10초 미응답 시 자동 합격 처리 |
| 본 게임 컨트롤 매핑 변경 시 튜토리얼 미반영 | UPM 패키지 분리 (4.1 (B)) |
