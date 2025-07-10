using UnityEngine;
using System.Reflection;

public class For_Char_Test : MonoBehaviour
{
    [SerializeField] private Human _human;
    [SerializeField] private string functionName = "TakeDamage"; // 인스펙터에서 입력
    [SerializeField] private object[] parameters = new object[] { 10f }; // 필요하면 하드코딩

    public void RunFunc()
    {
        if (_human == null)
        {
            Debug.LogError("Human is not assigned.");
            return;
        }

        MethodInfo method = _human.GetType().GetMethod(functionName);
        if (method != null)
        {
            method.Invoke(_human, parameters);
        }
        else
        {
            Debug.LogError($"Method {functionName} not found on {_human.GetType()}.");
        }
    }

    public void OnMouseDown()
    {
        Debug.Log($"Mouse down on {gameObject.name}. Running function {functionName}.");
        RunFunc();
    }

}
