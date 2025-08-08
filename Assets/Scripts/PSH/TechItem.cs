using System;
using System.Collections.Generic;

[Serializable]
public class MaterialRequirement
{
    public string materialName;
    public int amount;
}

[Serializable]
public class TechItem
{
    public string name;
    public string condition;               // ��: "����(��) ����", "����(��) ����, ���(��) ����"
    public int timeRequired;
    public List<MaterialRequirement> materials;
}
