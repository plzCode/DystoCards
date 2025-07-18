using System.Collections.Generic;
using UnityEngine;

public class TechLoader : MonoBehaviour
{

    public static TechLoader Instance { get; private set; }

    public TechData techData;
    public HashSet<string> researchedItems = new HashSet<string>();

    private void Awake()
    {
        // �̱��� �ߺ� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ���� �ν��Ͻ� �� ����
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ ���� (����)
    }

    public void Start()
    {
        // JSON �ε�
        TextAsset jsonFile = Resources.Load<TextAsset>("techData");
        techData = JsonUtility.FromJson<TechData>(jsonFile.text);

        // ���� �Ϸ� ����
        
        //researchedItems.Add("����(��)");
        researchedItems.Add("���(��)");

        // ��� ��� ���� ���� ���� Ȯ��
        foreach (var item in techData.tools)
        {
            Debug.Log($"{item.name} ���� ����? �� {CanResearch(item)}");
        }

        // �ʿ��� ��� ��� ����
        //foreach (var item in techData.tools)
        //{
        //    if (item.materials != null)
        //    {
        //        foreach (var mat in item.materials)
        //            Debug.Log($"{item.name} �ʿ� ���: {mat.materialName} x{mat.amount}");
        //    }
        //}
    }

    public bool CanResearch(TechItem item)
    {
        // ������ ���ų� "X"�̸� �ٷ� ���� ����
        if (string.IsNullOrEmpty(item.condition) || item.condition.Trim() == "X")
            return true;

        // ���� ������ ��� ��ǥ�� ���� ó��
        string[] conditions = item.condition.Split(',');

        foreach (string condition in conditions)
        {
            string trimmed = condition.Replace("����", "").Trim();
            if (!researchedItems.Contains(trimmed))
            {
                return false;
            }
        }

        return true;
    }

    // ���� �Ϸ� �Լ�
    public void CompleteResearch(string itemName)
    {
        researchedItems.Add(itemName);
        Debug.Log($"[���� �Ϸ�] {itemName}");
    }


}
