using MoreMountains.Feedbacks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GradeRecorder : MonoBehaviour
{
    public static GradeRecorder Instance { get; private set; }

    public float survivalCount;
    public float monsterKillCount;
    public float exploreCount;
    public float combinationCount;
    public float recipeOpenCount;
    public float humanDieCount;
    
    private Dictionary<string, float> gradeTable = new Dictionary<string, float>();

    [SerializeField] private GameObject GradeUI;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // �̸� ���� ������ ����
        gradeTable["����"] = 10f;
        gradeTable["���� óġ"] = 10f;
        gradeTable["Ž�� Ƚ��"] = 5f;
        gradeTable["���� Ƚ��"] = 1f;
        gradeTable["������ ���� Ƚ��"] = 3f;
        gradeTable["��� Ƚ��"] = -10f; // ��: ����� ����

    }

    
    /// <summary>
    /// ���� �����͸� ��ȯ (UI�� �� �����͸� �޾Ƽ� ǥ��)
    /// </summary>
    public List<GradeData> GetAllGrades()
    {
        List<GradeData> grades = new List<GradeData>();

        foreach (var entry in gradeTable)
        {
            string name = entry.Key;
            float scorePerUnit = entry.Value;
            float count = GetCountByName(name);
            float totalScore = count * scorePerUnit;

            grades.Add(new GradeData(name, count, totalScore));
        }

        return grades;
    }
        

    // �̸��� ���� �ش� ī��Ʈ�� ��ȯ
    private float GetCountByName(string name)
    {
        return name switch
        {
            "����" => TurnManager.Instance.TurnCount,
            "���� óġ" => monsterKillCount,
            "Ž�� Ƚ��" => exploreCount,
            "���� Ƚ��" => combinationCount,
            "������ ���� Ƚ��" => recipeOpenCount,
            "��� Ƚ��" => humanDieCount,
            _ => 0
        };
    }

    /// <summary>
    /// ���� ���� ������ ����ü
    /// </summary>
    public struct GradeData
    {
        public string name;
        public float count;
        public float totalScore;

        public GradeData(string name, float count, float totalScore)
        {
            this.name = name;
            this.count = count;
            this.totalScore = totalScore;
        }
    }
}
