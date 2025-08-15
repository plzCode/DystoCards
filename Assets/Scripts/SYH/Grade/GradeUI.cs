using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GradeRecorder;

public class GradeUI : MonoBehaviour
{
    [SerializeField] private GameObject gradePrefab;
    [SerializeField] private GameObject gridContent;
    [SerializeField] private TextMeshProUGUI totalText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowGrades();
        }
    }
    public void ShowGrades()
    {
        

        float totalGrade = 0f;
        List<GradeData> grades = GradeRecorder.Instance.GetAllGrades();

        foreach (var grade in grades)
        {
            GameObject obj = Instantiate(gradePrefab, gridContent.transform);
            obj.GetComponent<GradePrefab>().SetGrade(grade.name, grade.count, grade.totalScore);
            totalGrade += grade.totalScore;
        }

        totalText.text = totalGrade.ToString();
    }
}
