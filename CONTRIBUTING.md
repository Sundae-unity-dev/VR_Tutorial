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

### 포맷
```
v버전] 타입. 설명 (영어)
```

### 예시
```
v0.0.1] feat. Add VR locomotion system
v0.0.1] fix. Resolve camera jitter in VR mode
v0.0.2] edit. Adjust hand interaction grab distance
v0.0.2] refactor. Simplify interaction manager structure
v0.0.2] asset. Update environment prefabs
v0.0.3] docs. Update README setup guide
v0.0.3] chore. Configure Git LFS for binary assets
```

### 타입 정의

| 타입 | 용도 |
|------|------|
| `feat` | 새 기능 추가 |
| `fix` | 버그 수정 |
| `edit` | 기존 기능 수정/조정 |
| `refactor` | 코드 구조 개선 (동작 변화 없음) |
| `asset` | 씬·프리팹·텍스처 등 에셋 작업 |
| `docs` | 문서 수정 |
| `chore` | 빌드·설정 등 기타 작업 |

### 버전 규칙
```
v메이저.마이너.패치
  │      │     └── 버그 수정, 소규모 수정
  │      └──────── 기능 추가, 씬 구성 변경
  └─────────────── 대규모 구조 변경, 마일스톤
```
