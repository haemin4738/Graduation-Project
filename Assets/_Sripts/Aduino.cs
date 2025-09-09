using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // 시리얼 통신을 위한 네임스페이스
using TempleRun;
using TempleRun.Player;
using System.Diagnostics;

// 아두이노와의 통신을 통해 날씨 정보, 바닥 재질 및 점프 명령을 처리하는 클래스
public class Aduino : MonoBehaviour
{
    SerialPort arduino = new SerialPort("COM3", 9600); // COM3 포트로 9600 baudrate 시리얼 통신
    public string data; // 아두이노로부터 받은 데이터 문자열

    private WeatherManager weatherManager; // 날씨를 제어하는 매니저
    private TileSpawner tileSpawner; // 타일 생성 관리 컴포넌트
    private PlayerController playerController; // 플레이어 제어 컴포넌트

    // 시작 시 컴포넌트 초기화 및 아두이노 포트 열기
    void Start()
    {
        InitializeComponents(); // WeatherManager 및 TileSpawner 참조

        try
        {
            arduino.Open(); // 아두이노 포트 열기
            Debug.Log("Arduino port opened.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error opening Arduino port: " + e.Message);
        }

        // PlayerController 컴포넌트 찾기
        playerController = GetComponent<PlayerController>();

        // 디버그용 로그
        if (weatherManager != null)
            Debug.Log("WeatherManager component found.");
        else
            Debug.LogWarning("WeatherManager component not found.");

        if (tileSpawner != null)
            Debug.Log("TileSpawner component found.");
        else
            Debug.LogWarning("TileSpawner component not found.");

        if (playerController != null)
            Debug.Log("PlayerController component found.");
        else
            Debug.LogWarning("PlayerController component not found.");
    }

    // 컴포넌트들 참조 초기화 함수
    private void InitializeComponents()
    {
        // 타일 생성기 컴포넌트 찾기
        GameObject spawnManager = GameObject.Find("SpawnManager");
        if (spawnManager != null)
        {
            tileSpawner = spawnManager.GetComponent<TileSpawner>();
            if (tileSpawner == null)
            {
                Debug.LogError("SpawnManager 오브젝트에 TileSpawner 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("SpawnManager 오브젝트를 찾을 수 없습니다.");
        }

        // 날씨 매니저 컴포넌트 찾기
        Transform weatherManagerTransform = transform.Find("WeatherManager");
        if (weatherManagerTransform != null)
        {
            weatherManager = weatherManagerTransform.GetComponent<WeatherManager>();
            if (weatherManager == null)
            {
                Debug.LogError("WeatherManager 오브젝트에 WeatherManager 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("WeatherManager 자식 오브젝트를 찾을 수 없습니다.");
        }
    }

    // 매 프레임마다 아두이노 입력을 수신하고 처리
    void Update()
    {
        if (arduino.IsOpen)
        {
            try
            {
                data = arduino.ReadLine(); // 한 줄 데이터 읽기

                string[] parts = data.Split(','); // 쉼표 기준으로 데이터 분리
                if (parts.Length == 2)
                {
                    // 날씨 및 타일 정보 가져오기
                    int currentWeather = 0;
                    if (weatherManager != null)
                    {
                        currentWeather = weatherManager.current_weather;
                        Debug.Log("Current Weather: " + currentWeather);
                    }

                    int floorMaterials = 0;
                    if (tileSpawner != null)
                    {
                        floorMaterials = tileSpawner.currentMaterialIndex;
                        Debug.Log("Floor Materials Index: " + floorMaterials);
                    }

                    // 디버그 출력
                    Debug.Log(currentWeather + " " + floorMaterials + "================================");
                    Debug.Log(currentWeather + floorMaterials + "************************************");

                    // 조건에 따라 두 번째 값 설정 (예: LED 제어 등)
                    if (currentWeather + floorMaterials >= 1)
                        parts[1] = "1";
                    else
                        parts[1] = "0";

                    // 다시 문자열로 조합하여 아두이노에 전송
                    data = string.Join(",", parts);
                    arduino.WriteLine(data);

                    // 첫 번째 값이 10 이상일 경우 점프 명령으로 간주
                    if (int.TryParse(parts[0], out int result) && result >= 10)
                    {
                        Debug.Log("Jump command received from Arduino");
                        if (playerController != null)
                        {
                            Debug.Log("Calling Jump method on PlayerController");
                            playerController.Jump(); // 점프 실행
                        }
                        else
                        {
                            Debug.LogWarning("PlayerController component is not assigned.");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error reading from Arduino: " + e.Message);
            }
        }

        // 디버그용 데이터 확인
        // Debug.Log("Data from Arduino: " + data);
    }
}
