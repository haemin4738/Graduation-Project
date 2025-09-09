using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스

// 게임 종료 시 UI 처리 및 점수 관리, 재시작 등을 담당하는 클래스
public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas; // 게임 오버 시 표시할 UI 캔버스

    [SerializeField]
    private TextMeshProUGUI FinalScoreText; // (예정된) 최종 점수를 표시할 텍스트 (현재 사용되지 않음)
    [SerializeField]
    private TextMeshProUGUI scoreText; // 현재 점수를 표시할 텍스트

    private int score = 0; // 내부적으로 저장할 점수 값

    // 게임이 종료되었을 때 호출되는 함수
    public void StopGame(int score)
    {
        gameOverCanvas.SetActive(true); // 게임 오버 UI 활성화
        this.score = score; // 전달받은 점수를 저장
        scoreText.text = score.ToString(); // 점수를 텍스트에 표시
    }

    // 게임 재시작 버튼을 누르면 호출될 함수 (현재 구현되지 않음)
    public void RestartLevel()
    {
        // 추후 구현 예정
    }

    // 점수 서버 전송 또는 기록용 함수 (현재 구현되지 않음)
    public void SubmitScore()
    {
        // 추후 구현 예정
    }

    // 경험치 추가 기능 (현재 구현되지 않음)
    public void AddXP(int score)
    {
        // 추후 구현 예정
    }

    // 현재 씬을 다시 불러와 게임 재시작
    public void ReloadScene()
    {
        // 현재 활성화된 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
