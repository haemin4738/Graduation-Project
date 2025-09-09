using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스

// 점수 UI를 업데이트하는 역할을 하는 클래스
public class ScoreUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText; // 화면에 점수를 표시할 TextMeshProUGUI 컴포넌트 참조

    // 외부에서 점수를 전달받아 UI 텍스트를 갱신하는 함수
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString(); // 숫자를 문자열로 변환해 텍스트에 표시
    }
}
