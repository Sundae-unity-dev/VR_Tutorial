# 브랜치 전략

## 브랜치 구조

```
main
└── develop
    ├── feature/기능명
    ├── fix/버그명
    └── hotfix/긴급수정명   ← main에서 분기
```

| 브랜치 | 용도 | 병합 대상 |
|--------|------|-----------|
| `main` | 안정 배포 버전 | — (PR만 허용) |
| `develop` | 통합 개발 브랜치 | main |
| `feature/*` | 새 기능 개발 | develop |
| `fix/*` | 버그 수정 | develop |
| `hotfix/*` | 긴급 수정 | main + develop |

## 브랜치 네이밍 규칙

```bash
feature/locomotion-system
feature/hand-interaction
fix/vr-camera-jitter
hotfix/build-crash
```

## 작업 흐름

### 일반 기능 개발
```bash
git checkout develop
git pull origin develop
git checkout -b feature/기능명

# 작업 후
git push origin feature/기능명
# → GitHub에서 develop으로 PR 생성
```

### 긴급 수정 (hotfix)
```bash
git checkout main
git pull origin main
git checkout -b hotfix/수정명

# 수정 후
git push origin hotfix/수정명
# → GitHub에서 main + develop 둘 다 PR 생성
```

## 커밋 메시지 규칙

```
feat: VR 이동 시스템 추가
fix: 카메라 떨림 현상 수정
refactor: 인터랙션 매니저 구조 개선
asset: 환경 프리팹 업데이트
docs: README 수정
```
