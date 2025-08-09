using System.Collections.Generic;
using UnityEngine;

public class TechLoader : MonoBehaviour
{

    public static TechLoader Instance { get; private set; }

    public TechData techData;
    public HashSet<string> researchedItems = new HashSet<string>();

    private void Awake()
    {
        // 싱글톤 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 기존 인스턴스 외 제거
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 유지 (선택)
    }

    public void Start()
    {
        // JSON 로드
        TextAsset jsonFile = Resources.Load<TextAsset>("techData");
        techData = JsonUtility.FromJson<TechData>(jsonFile.text);

        // 연구 완료 예시
        
        //researchedItems.Add("도끼(돌)");
        researchedItems.Add("곡괭이(돌)");

        // 모든 장비 제작 가능 여부 확인
        foreach (var item in techData.tools)
        {
            Debug.Log($"{item.name} 연구 가능? → {CanResearch(item)}");
        }

        // 필요한 재료 출력 예시
        //foreach (var item in techData.tools)
        //{
        //    if (item.materials != null)
        //    {
        //        foreach (var mat in item.materials)
        //            Debug.Log($"{item.name} 필요 재료: {mat.materialName} x{mat.amount}");
        //    }
        //}
    }

    public bool CanResearch(TechItem item)
    {
        // 조건이 없거나 "X"이면 바로 연구 가능
        if (string.IsNullOrEmpty(item.condition) || item.condition.Trim() == "X")
            return true;

        // 여러 조건일 경우 쉼표로 나눠 처리
        string[] conditions = item.condition.Split(',');

        foreach (string condition in conditions)
        {
            string trimmed = condition.Replace("연구", "").Trim();
            if (!researchedItems.Contains(trimmed))
            {
                return false;
            }
        }

        return true;
    }

    // 연구 완료 함수
    public void CompleteResearch(string itemName)
    {
        researchedItems.Add(itemName);
        Debug.Log($"[연구 완료] {itemName}");
    }


}
