# VR_Tutorial

Unity XR Interaction Toolkit 기반 VR 입문자용 튜토리얼.
방문자가 본 VR 게임 진입 전 5~10분 안에 그랩 · 텔레포트 · UI 인터랙션을 익히도록 돕는 온보딩 씬.

## 타겟 환경
- Unity 2022.3 LTS
- XR Interaction Toolkit 2.5+
- OpenXR Plugin
- 1차 디바이스: Meta Quest 2 / 3

## 문서
- [기획서](docs/PROPOSAL.md) — 목적, 학습 목표, 마일스톤, 성공 지표
- [씬 구조 설계](docs/SCENE_STRUCTURE.md) — 씬 흐름, 폴더 구조, 자산 공유 전략
- [튜토리얼 시나리오](docs/TUTORIAL_SCENARIO.md) — 8단계 미션 상세

## 빌드 / 배포
Meta Quest용 `.apk` 빌드를 [GitHub Releases](https://github.com/Sundae-unity-dev/VR_Tutorial/releases)에 업로드한다.

### 빌드 절차 (Quest)
1. Unity → `File > Build Settings`
2. Platform: **Android** 로 전환 (`Switch Platform`)
3. Texture Compression: **ASTC**
4. `Edit > Project Settings > XR Plug-in Management > Android` 탭에서 **OpenXR + Meta Quest Feature Group** 활성화
5. `Player Settings > Other Settings`
   - Minimum API Level: **Android 10 (API 29)**
   - Target Architectures: **ARM64** 만 체크
   - Scripting Backend: **IL2CPP**
6. `Build` → `VR_Tutorial_v{버전}.apk` 산출

### 설치 (방문자/현장 운영자)
- Quest 헤드셋을 PC에 USB-C 연결 후 SideQuest 또는 `adb install VR_Tutorial.apk`
- 또는 Meta Quest Developer Hub 의 "Apps" → "Add Build" 로 드래그앤드롭

### 릴리스 명명 규칙
- 태그: `v0.1.0`, `v0.2.0`, ...
- 산출물: `VR_Tutorial_v0.1.0.apk` (Quest 2/3 공통)

## 라이선스
[MIT](LICENSE)
