using TMPro;
using UnityEngine;

public class GradePrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI gradeText;



    public void SetGrade(string _name,float _count,float _grade)
    {
        titleText.text = _name + "(" + _count + ")";
        gradeText.text = _grade.ToString();
    }

}
