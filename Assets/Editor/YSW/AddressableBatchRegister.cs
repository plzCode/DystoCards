using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using System.Linq;

public class AddressableBatchRegister : EditorWindow
{
    string folderPath = "Assets/Audio"; // 등록할 폴더 경로
    string groupName = "SFX";
    string labelName = "SFX";

    [MenuItem("Tools/Addressables/Batch Register Audio")]
    public static void ShowWindow()
    {
        var window = GetWindow<AddressableBatchRegister>(true, "Addressable Batch Register");
        window.minSize = new Vector2(350, 150);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Register Addressables", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("Folder Path:", folderPath);
        groupName = EditorGUILayout.TextField("Group Name:", groupName);
        labelName = EditorGUILayout.TextField("Label Name:", labelName);

        GUILayout.Space(10);
        if (GUILayout.Button("Register All in Folder", GUILayout.Height(30)))
        {
            RegisterAllAssetsInFolder();
        }
    }

    void RegisterAllAssetsInFolder()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found. Make sure Addressables is set up.");
            return;
        }

        var group = settings.FindGroup(groupName) ?? settings.DefaultGroup;

        // 라벨 없으면 생성
        if (!settings.GetLabels().Contains(labelName))
        {
            settings.AddLabel(labelName);
            Debug.Log($"✅ Created new label: {labelName}");
        }

        string[] assetGUIDs = AssetDatabase.FindAssets("", new[] { folderPath });
        foreach (var guid in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var entry = settings.CreateOrMoveEntry(guid, group);
            string key = Path.GetFileNameWithoutExtension(assetPath);
            entry.address = key;
            entry.SetLabel(labelName, true);
        }

        Debug.Log($"✅ Registered {assetGUIDs.Length} assets from '{folderPath}' to group '{groupName}' with label '{labelName}'.");
    }
}