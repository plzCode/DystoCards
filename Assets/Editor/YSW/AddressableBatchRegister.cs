using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.IO;

public class AddressableBatchRegister : EditorWindow
{
    [SerializeField] public string folderPath = "Assets/Audio"; // 등록할 폴더 경로
    [SerializeField] public string groupName = "SFX";

    [MenuItem("Tools/Addressables/Batch Register Audio")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AddressableBatchRegister));
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Register Addressables", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("Folder Path:", folderPath);
        groupName = EditorGUILayout.TextField("Group Name:", groupName);

        if (GUILayout.Button("Register All in Folder"))
        {
            RegisterAllAssetsInFolder();
        }
    }

    void RegisterAllAssetsInFolder()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.FindGroup(groupName) ?? settings.DefaultGroup;

        string[] assetGUIDs = AssetDatabase.FindAssets("", new[] { folderPath });
        foreach (var guid in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var entry = settings.CreateOrMoveEntry(guid, group);
            string key = Path.GetFileNameWithoutExtension(assetPath);
            entry.address = key;
        }

        Debug.Log($"Registered {assetGUIDs.Length} assets from {folderPath} to Addressables in group '{groupName}'.");
    }
}
