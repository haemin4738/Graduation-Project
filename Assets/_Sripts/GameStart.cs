using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스
using UnityEngine.UI; // UI(Button) 사용을 위한 네임스페이스

// 버튼 클릭 시 게임을 시작(씬 전환)하는 스크립트
public class GameStart : MonoBehaviour
{
    void Start()
    {
        // 이 스크립트가 붙은 오브젝트의 Button 컴포넌트를 가져와 클릭 이벤트에 StartGame 함수 등록
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    // 버튼 클릭 시 호출되는 함수
    public void StartGame()
    {
        // "SampleScene"이라는 이름의 씬으로 전환
        SceneManager.LoadScene("SampleScene");

        // 콘솔에 로그 출력 (디버깅용)
        Debug.Log("game start");
    }
}
