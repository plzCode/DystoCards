using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GradeRecorder : MonoBehaviour
{
    public float survivalCount;
    public float monsterKillCount;
    public float exploreCount;
    public float combinationCount;
    public float recipeOpenCount;
    public float humanDieCount;

    

    [SerializeField] private GameObject gradePrefab;
    [SerializeField] private GameObject gridContent;
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private float totalGrade;

    private Dictionary<string, float> gradeTable = new Dictionary<string, float>();

    private void Awake()
    {
        // �̸� ���� ������ ����
        gradeTable["����"] = 10f;
        gradeTable["���� óġ"] = 10f;
        gradeTable["Ž�� Ƚ��"] = 5f;
        gradeTable["���� Ƚ��"] = 1f;
        gradeTable["������ ���� Ƚ��"] = 3f;
        gradeTable["��� Ƚ��"] = -10f; // ��: ����� ����

        SetGradeUI();
    }

    private void SetGradeUI()
    {
        // �ݺ������� Dictionary�� �׸��� ��ȸ
        foreach (var entry in gradeTable)
        {
            // �׸� �̸��� ī��Ʈ�� ��������
            string name = entry.Key;
            float scorePerUnit = entry.Value;
            float count = GetCountByName(name);

            // �� ���� ���
            float totalScore = count * scorePerUnit;

            // ������ ����
            GameObject obj = Instantiate(gradePrefab, gridContent.transform);

            // GradePrefab ��ũ��Ʈ�� �� ����
            obj.GetComponent<GradePrefab>().SetGrade(name, count, totalScore);

            totalGrade += totalScore;
        }

        totalText.text = totalGrade.ToString();
    }

    // �̸��� ���� �ش� ī��Ʈ�� ��ȯ
    private float GetCountByName(string name)
    {
        return name switch
        {
            "����" => survivalCount,
            "���� óġ" => monsterKillCount,
            "Ž�� Ƚ��" => exploreCount,
            "���� Ƚ��" => combinationCount,
            "������ ���� Ƚ��" => recipeOpenCount,
            "��� Ƚ��" => humanDieCount,
            _ => 0
        };
    }
}
