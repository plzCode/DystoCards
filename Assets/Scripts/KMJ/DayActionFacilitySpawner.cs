using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DayActionFacilitySpawner : MonoBehaviour
{
    [Serializable]
    public struct SpawnSpec
    {
        [Tooltip("true면 Card Id, false면 Card Name을 사용")]
        public bool useId;
        [Tooltip("Card Id 또는 Card Name")]
        public string idOrName;
        [Tooltip("월드 좌표(씬 기준)")]
        public Vector3 worldPos;
    }

    [Header("DayAction에 배치할 시설 카드들")]
    public List<SpawnSpec> facilities = new();

    [Header("DayAction에 배치할 캐릭터(예: Human)")]
    public List<SpawnSpec> characters = new();

    [Header("씬 실행과 동시에 스폰할지 여부")]
    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart) SpawnAll();
    }

    /// <summary>시설 + 캐릭터 전부 생성</summary>
    public void SpawnAll()
    {
        if (CardManager.Instance == null)
        {
            Debug.LogError("[Spawner] CardManager.Instance가 없습니다.");
            return;
        }

        // 시설
        foreach (var s in facilities)
            SafeSpawn(s, isFacility: true);

        // 캐릭터
        foreach (var s in characters)
            SafeSpawn(s, isFacility: false);
    }

    void SafeSpawn(SpawnSpec s, bool isFacility)
    {
        Card2D spawned = null;
        if (s.useId) spawned = CardManager.Instance.SpawnCardById(s.idOrName, s.worldPos);
        else spawned = CardManager.Instance.SpawnCardByName(s.idOrName, s.worldPos);

        if (!spawned)
        {
            Debug.LogError($"[Spawner] 스폰 실패: {(s.useId ? "ID" : "Name")}='{s.idOrName}' @ {s.worldPos}");
            return;
        }

        Debug.Log($"[Spawner] 카드 소환: {spawned.name} @ {s.worldPos}");
    }
}