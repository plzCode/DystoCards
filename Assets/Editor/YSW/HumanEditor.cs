using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Human))]
public class HumanEditor : Editor
{
    private ReorderableList equipmentList;

    private void OnEnable()
    {
        equipmentList = new ReorderableList(
            serializedObject,
            serializedObject.FindProperty("equipmentSlotList"),
            true, true, true, true);

        equipmentList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Equipment Slots");
        };

        equipmentList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = equipmentList.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2;
            float halfWidth = rect.width / 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("slot"), GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + halfWidth + 5, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("equipment"), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector(); // other fields (health, stamina...)

        EditorGUILayout.Space();
        equipmentList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
