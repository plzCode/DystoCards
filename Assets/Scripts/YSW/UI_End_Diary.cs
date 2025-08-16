using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_End_Diary : MonoBehaviour
{
    public GameObject story;
    public Transform contents;
    public void OnEnable()
    {
        //초기화
        foreach(Transform child in contents)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<(int, string, int)> lines = Recorder.Instance.GetAllStory();
        foreach (var line in lines)
        {
            GameObject obj = Instantiate(story, contents).gameObject;
            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();

            if (tmp != null)
            {
                tmp.text = $"{line.Item1}일차, {line.Item2}";
            }
        }
    }
}
