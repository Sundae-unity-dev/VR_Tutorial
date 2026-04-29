using UnityEngine;

namespace VRTutorial
{
    /// <summary>
    /// 씬 전환 후에도 유지되는 튜토리얼 세션 데이터.
    /// 점수, 완료 여부, 플레이어 설정을 저장한다.
    /// </summary>
    public class TutorialSession : MonoBehaviour
    {
        public static TutorialSession Instance { get; private set; }

        // 존별 점수
        public int Zone1Score { get; private set; }
        public int Zone2Score { get; private set; }
        public int Zone3Score { get; private set; }
        public int TotalScore => Zone1Score + Zone2Score + Zone3Score;

        // 존 완료 여부
        public bool Zone1Completed { get; private set; }
        public bool Zone2Completed { get; private set; }
        public bool Zone3Completed { get; private set; }

        // 플레이어 설정
        [Header("Player Settings")]
        [SerializeField] float _moveSpeed = 1f;
        [SerializeField] bool _snapTurn = false;
        [SerializeField] float _snapTurnAngle = 45f;

        public float MoveSpeed
        {
            get => _moveSpeed;
            set { _moveSpeed = value; OnSettingsChanged?.Invoke(); }
        }

        public bool SnapTurn
        {
            get => _snapTurn;
            set { _snapTurn = value; OnSettingsChanged?.Invoke(); }
        }

        public float SnapTurnAngle
        {
            get => _snapTurnAngle;
            set { _snapTurnAngle = value; OnSettingsChanged?.Invoke(); }
        }

        public event System.Action OnSettingsChanged;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddScore(int zoneIndex, int points)
        {
            switch (zoneIndex)
            {
                case 0: Zone1Score += points; break;
                case 1: Zone2Score += points; break;
                case 2: Zone3Score += points; break;
            }
        }

        public void CompleteZone(int zoneIndex)
        {
            switch (zoneIndex)
            {
                case 0: Zone1Completed = true; break;
                case 1: Zone2Completed = true; break;
                case 2: Zone3Completed = true; break;
            }
        }

        public void ResetSession()
        {
            Zone1Score = Zone2Score = Zone3Score = 0;
            Zone1Completed = Zone2Completed = Zone3Completed = false;
        }
    }
}
