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
        // 미리 점수 기준을 저장
        gradeTable["생존"] = 10f;
        gradeTable["몬스터 처치"] = 10f;
        gradeTable["탐험 횟수"] = 5f;
        gradeTable["조합 횟수"] = 1f;
        gradeTable["레시피 오픈 횟수"] = 3f;
        gradeTable["사망 횟수"] = -10f; // 예: 사망은 감점

        SetGradeUI();
    }

    private void SetGradeUI()
    {
        // 반복문으로 Dictionary의 항목을 순회
        foreach (var entry in gradeTable)
        {
            // 항목 이름과 카운트를 가져오기
            string name = entry.Key;
            float scorePerUnit = entry.Value;
            float count = GetCountByName(name);

            // 총 점수 계산
            float totalScore = count * scorePerUnit;

            // 프리팹 생성
            GameObject obj = Instantiate(gradePrefab, gridContent.transform);

            // GradePrefab 스크립트에 값 전달
            obj.GetComponent<GradePrefab>().SetGrade(name, count, totalScore);

            totalGrade += totalScore;
        }

        totalText.text = totalGrade.ToString();
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
}
