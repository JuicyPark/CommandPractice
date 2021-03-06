﻿using UnityEngine;
using System.Collections;
using System;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    BulletLauncher launcherPrefab;
    BulletLauncher launcher;

    [SerializeField]
    Transform launcherLocator;

    [SerializeField]
    Building buildingPrefab;

    [SerializeField]
    Transform[] buildingLocators;

    [SerializeField]
    Missile missilePrefab;

    [SerializeField]
    DestroyEffect effectPrefab;

    [SerializeField]
    int maxMissileCount = 20;

    [SerializeField]
    float missileSpawnInterval = 0.5f;

    [SerializeField]
    int scorePerMissile = 50;

    [SerializeField]
    int scorePerBuilding = 5000;

    [SerializeField]
    UIRoot uIRoot;

    bool isAllBuildingDestroyed;

    public Action<bool, int> GameEnded;

    MouseGameController mouseGameController;
    BuildingManager buildingManager;
    TimeManager timeManager;
    MissileManager missileManager;
    ScoreManager scoreManager;

    void Start()
    {
        launcher = Instantiate(launcherPrefab);
        launcher.transform.position = launcherLocator.position;

        mouseGameController = gameObject.AddComponent<MouseGameController>();

        buildingManager = new BuildingManager(buildingPrefab, buildingLocators, new Factory(effectPrefab));
        timeManager = gameObject.AddComponent<TimeManager>();
        missileManager = gameObject.AddComponent<MissileManager>();
        missileManager.Initialize(new Factory(missilePrefab), buildingManager, maxMissileCount, missileSpawnInterval);

        scoreManager = new ScoreManager(scorePerMissile, scorePerBuilding);

        BindEvents();
        timeManager.StartGame(1f);
    }

    void OnDestroy()
    {
        UnBindEvents();
    }

    void OnAllBuildingDestroyed()
    {
        // END
        isAllBuildingDestroyed = true;
        GameEnded?.Invoke(false, buildingManager.BuildingCount);
    }

    void BindEvents()
    {
        mouseGameController.FireButtonPressed += launcher.OnFireButtonPressed;
        timeManager.GameStarted += buildingManager.OnGameStarted;
        timeManager.GameStarted += launcher.OnGameStarted;
        timeManager.GameStarted += missileManager.OnGameStarted;
        timeManager.GameStarted += uIRoot.OnGameStarted;
        missileManager.missileDestroyed += scoreManager.OnMissileDestroyed;
        missileManager.AllMissilesDestroyed += OnAllBuildingDestroyed;
        scoreManager.ScoreChanged += uIRoot.OnScoreChanged;
        buildingManager.AllBuildingsDestroyed += OnAllBuildingDestroyed;
    }

    void UnBindEvents()
    {
        mouseGameController.FireButtonPressed -= launcher.OnFireButtonPressed;
        timeManager.GameStarted -= buildingManager.OnGameStarted;
        timeManager.GameStarted -= launcher.OnGameStarted;
        timeManager.GameStarted -= missileManager.OnGameStarted;
        timeManager.GameStarted -= uIRoot.OnGameStarted;
        missileManager.missileDestroyed -= scoreManager.OnMissileDestroyed;
        missileManager.AllMissilesDestroyed -= OnAllBuildingDestroyed;
        scoreManager.ScoreChanged += uIRoot.OnScoreChanged;
        buildingManager.AllBuildingsDestroyed -= OnAllBuildingDestroyed;
    }

    IEnumerator CDelayedGameEnded()
    {
        yield return null;

        if(!isAllBuildingDestroyed)
        {
            // WIN
        }
    }
}
