﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileManager : MonoBehaviour
{
    Factory missileFactory;
    BuildingManager buildingManager;

    int maxMissileCount = 20;
    int currentMissileCount;

    float missileSpawnInterval = 0.5f;

    Coroutine spawningMissile;

    bool isInitialize = false;

    public void Initialize(Factory missileFactory, BuildingManager buildingManager, int maxMissileCount, float missileSpawnInterval)
    {
        if (isInitialize)
            return;

        this.missileFactory = missileFactory;
        this.buildingManager = buildingManager;
        this.maxMissileCount = maxMissileCount;
        this.missileSpawnInterval = missileSpawnInterval;

        isInitialize = true;
    }

    public void OnGameStarted()
    {
        currentMissileCount = 0;
        spawningMissile = StartCoroutine(CAutoSpawnMissile());
    }

    void SpawnMissile()
    {
        RecycleObject missile = missileFactory.Get();
        missile.Activate(GetMissileSpawnPosition(), buildingManager.GetRandomBuildingPosition());
        missile.Destroyed += OnMissileDestroyed;
        currentMissileCount++;
    }

    Vector3 GetMissileSpawnPosition()
    {
        Vector3 spawnPostion = Vector3.zero;
        spawnPostion.x = Random.Range(0f, 1f);
        spawnPostion.y = 1f;

        spawnPostion = Camera.main.ViewportToWorldPoint(spawnPostion);
        spawnPostion.z = 0f;
        return spawnPostion;
    }

    void OnMissileDestroyed(RecycleObject missile)
    {
        missile.Destroyed -= OnMissileDestroyed;
        missileFactory.Restore(missile);
    }

    IEnumerator CAutoSpawnMissile()
    {
        while (currentMissileCount < maxMissileCount)
        {
            yield return new WaitForSeconds(missileSpawnInterval);

            if (!buildingManager.HasBuilding)
            {
                yield break;
            }

            SpawnMissile();
        }
    }
}