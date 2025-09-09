using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // �ø��� ����� ���� ���ӽ����̽�
using TempleRun;
using TempleRun.Player;
using System.Diagnostics;

// �Ƶ��̳���� ����� ���� ���� ����, �ٴ� ���� �� ���� ����� ó���ϴ� Ŭ����
public class Aduino : MonoBehaviour
{
    SerialPort arduino = new SerialPort("COM3", 9600); // COM3 ��Ʈ�� 9600 baudrate �ø��� ���
    public string data; // �Ƶ��̳�κ��� ���� ������ ���ڿ�

    private WeatherManager weatherManager; // ������ �����ϴ� �Ŵ���
    private TileSpawner tileSpawner; // Ÿ�� ���� ���� ������Ʈ
    private PlayerController playerController; // �÷��̾� ���� ������Ʈ

    // ���� �� ������Ʈ �ʱ�ȭ �� �Ƶ��̳� ��Ʈ ����
    void Start()
    {
        InitializeComponents(); // WeatherManager �� TileSpawner ����

        try
        {
            arduino.Open(); // �Ƶ��̳� ��Ʈ ����
            Debug.Log("Arduino port opened.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error opening Arduino port: " + e.Message);
        }

        // PlayerController ������Ʈ ã��
        playerController = GetComponent<PlayerController>();

        // ����׿� �α�
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

    // ������Ʈ�� ���� �ʱ�ȭ �Լ�
    private void InitializeComponents()
    {
        // Ÿ�� ������ ������Ʈ ã��
        GameObject spawnManager = GameObject.Find("SpawnManager");
        if (spawnManager != null)
        {
            tileSpawner = spawnManager.GetComponent<TileSpawner>();
            if (tileSpawner == null)
            {
                Debug.LogError("SpawnManager ������Ʈ�� TileSpawner ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("SpawnManager ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // ���� �Ŵ��� ������Ʈ ã��
        Transform weatherManagerTransform = transform.Find("WeatherManager");
        if (weatherManagerTransform != null)
        {
            weatherManager = weatherManagerTransform.GetComponent<WeatherManager>();
            if (weatherManager == null)
            {
                Debug.LogError("WeatherManager ������Ʈ�� WeatherManager ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("WeatherManager �ڽ� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // �� �����Ӹ��� �Ƶ��̳� �Է��� �����ϰ� ó��
    void Update()
    {
        if (arduino.IsOpen)
        {
            try
            {
                data = arduino.ReadLine(); // �� �� ������ �б�

                string[] parts = data.Split(','); // ��ǥ �������� ������ �и�
                if (parts.Length == 2)
                {
                    // ���� �� Ÿ�� ���� ��������
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

                    // ����� ���
                    Debug.Log(currentWeather + " " + floorMaterials + "================================");
                    Debug.Log(currentWeather + floorMaterials + "************************************");

                    // ���ǿ� ���� �� ��° �� ���� (��: LED ���� ��)
                    if (currentWeather + floorMaterials >= 1)
                        parts[1] = "1";
                    else
                        parts[1] = "0";

                    // �ٽ� ���ڿ��� �����Ͽ� �Ƶ��̳뿡 ����
                    data = string.Join(",", parts);
                    arduino.WriteLine(data);

                    // ù ��° ���� 10 �̻��� ��� ���� ������� ����
                    if (int.TryParse(parts[0], out int result) && result >= 10)
                    {
                        Debug.Log("Jump command received from Arduino");
                        if (playerController != null)
                        {
                            Debug.Log("Calling Jump method on PlayerController");
                            playerController.Jump(); // ���� ����
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

        // ����׿� ������ Ȯ��
        // Debug.Log("Data from Arduino: " + data);
    }
}
