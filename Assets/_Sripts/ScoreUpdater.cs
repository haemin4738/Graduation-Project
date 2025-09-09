using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�

// ���� UI�� ������Ʈ�ϴ� ������ �ϴ� Ŭ����
public class ScoreUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText; // ȭ�鿡 ������ ǥ���� TextMeshProUGUI ������Ʈ ����

    // �ܺο��� ������ ���޹޾� UI �ؽ�Ʈ�� �����ϴ� �Լ�
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString(); // ���ڸ� ���ڿ��� ��ȯ�� �ؽ�Ʈ�� ǥ��
    }
}
