using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(RecipeCardData))]
public class RecipeCardDataEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        reorderableList = new ReorderableList(
            serializedObject,
            serializedObject.FindProperty("ingredients"),
            true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Ingredients");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float halfWidth = rect.width / 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("ingredient"), GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + halfWidth + 5, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("quantity"), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
                
        DrawDefaultInspector(); // result와 cardName 등 기본 필드
        reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}