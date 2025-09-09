using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� ���ӽ����̽�

// ���� ���� �� UI ó�� �� ���� ����, ����� ���� ����ϴ� Ŭ����
public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas; // ���� ���� �� ǥ���� UI ĵ����

    [SerializeField]
    private TextMeshProUGUI FinalScoreText; // (������) ���� ������ ǥ���� �ؽ�Ʈ (���� ������ ����)
    [SerializeField]
    private TextMeshProUGUI scoreText; // ���� ������ ǥ���� �ؽ�Ʈ

    private int score = 0; // ���������� ������ ���� ��

    // ������ ����Ǿ��� �� ȣ��Ǵ� �Լ�
    public void StopGame(int score)
    {
        gameOverCanvas.SetActive(true); // ���� ���� UI Ȱ��ȭ
        this.score = score; // ���޹��� ������ ����
        scoreText.text = score.ToString(); // ������ �ؽ�Ʈ�� ǥ��
    }

    // ���� ����� ��ư�� ������ ȣ��� �Լ� (���� �������� ����)
    public void RestartLevel()
    {
        // ���� ���� ����
    }

    // ���� ���� ���� �Ǵ� ��Ͽ� �Լ� (���� �������� ����)
    public void SubmitScore()
    {
        // ���� ���� ����
    }

    // ����ġ �߰� ��� (���� �������� ����)
    public void AddXP(int score)
    {
        // ���� ���� ����
    }

    // ���� ���� �ٽ� �ҷ��� ���� �����
    public void ReloadScene()
    {
        // ���� Ȱ��ȭ�� ���� �ٽ� �ε�
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
