using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� ���ӽ����̽�
using UnityEngine.UI; // UI(Button) ����� ���� ���ӽ����̽�

// ��ư Ŭ�� �� ������ ����(�� ��ȯ)�ϴ� ��ũ��Ʈ
public class GameStart : MonoBehaviour
{
    void Start()
    {
        // �� ��ũ��Ʈ�� ���� ������Ʈ�� Button ������Ʈ�� ������ Ŭ�� �̺�Ʈ�� StartGame �Լ� ���
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void StartGame()
    {
        // "SampleScene"�̶�� �̸��� ������ ��ȯ
        SceneManager.LoadScene("SampleScene");

        // �ֿܼ� �α� ��� (������)
        Debug.Log("game start");
    }
}
