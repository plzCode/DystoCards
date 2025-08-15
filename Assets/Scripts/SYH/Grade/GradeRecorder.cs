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

        // 미리 점수 기준을 저장
        gradeTable["생존"] = 10f;
        gradeTable["몬스터 처치"] = 10f;
        gradeTable["탐험 횟수"] = 5f;
        gradeTable["조합 횟수"] = 1f;
        gradeTable["레시피 오픈 횟수"] = 3f;
        gradeTable["사망 횟수"] = -10f; // 예: 사망은 감점

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GradeUI.gameObject.SetActive(true);
            GradeUI.GetComponent<GradeUI>().ShowGrades();
        }

        if (GradeUI.gameObject.activeSelf)
            {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    /// <summary>
    /// 점수 데이터를 반환 (UI가 이 데이터를 받아서 표시)
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
        

    // 이름에 따라 해당 카운트를 반환
    private float GetCountByName(string name)
    {
        return name switch
        {
            "생존" => survivalCount,
            "몬스터 처치" => monsterKillCount,
            "탐험 횟수" => exploreCount,
            "조합 횟수" => combinationCount,
            "레시피 오픈 횟수" => recipeOpenCount,
            "사망 횟수" => humanDieCount,
            _ => 0
        };
    }

    /// <summary>
    /// 개별 점수 데이터 구조체
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
