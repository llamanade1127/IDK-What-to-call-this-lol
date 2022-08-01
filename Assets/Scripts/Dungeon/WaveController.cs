using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = System.Random;


public class WaveController : MonoBehaviour
{
    public Wave[] waves;
    public Transform[] globalSpawnPoints;
    public int waveIndex;
    public State state;

    private List<GameObject> CurrentEnemies = new List<GameObject>();
    private Random rng = new Random();
    public void Update()
    {
        if (state == State.Spawning)
        {
            StartCoroutine(SpawnWave(waves[waveIndex]));
        }

        for (int i = 0; i < CurrentEnemies.Count; i++)
        {
            if (!CurrentEnemies[i])
                CurrentEnemies.Remove(CurrentEnemies[i]);
        }

        if (state == State.Waiting && CurrentEnemies.Count == 0)
        {
            waveIndex += 1;
            state = State.Spawning;
        }
    }
    
    public enum State
    {
        Counting,
        Waiting,
        Spawning
    }

    IEnumerator SpawnWave(Wave wave)
    {
        state = State.Spawning;

        for (int i = 0; i < wave.enemies.Length; i++)
        {
            StartCoroutine(SpawnEnemies(wave.enemies[i], wave));
        }
        
        state = State.Waiting;
        yield break;
    }

    IEnumerator SpawnEnemies(WaveEnemy enemy, Wave wave)
    {
        for (int i = 0; i < enemy.amount; i++)
        {
            if (wave.spawnPoints.Length == 0)
            {
                SpawnEnemy(enemy.enemy, globalSpawnPoints[rng.Next(globalSpawnPoints.Length)]);
            }
            else
            {
                SpawnEnemy(enemy.enemy, wave.spawnPoints[rng.Next(wave.spawnPoints.Length)]);
            }

            yield return new WaitForSeconds(enemy.tickTime / enemy.spawnPerTick);
        }
        
        yield break;
    }

    // private int[] GenerateRandomIntArray(int max, int length)
    // {
    //     int[] arr = new int[length];
    //     
    //     
    //     for (int i = 0; i < length; i++)
    //     {
    //             
    //     }
    // }

    private void SpawnEnemy(GameObject enemy, Transform transform)
    {
        CurrentEnemies.Add(Instantiate(enemy, transform));
    }
}






[Serializable]
public struct Wave
{
    public string name;
    public Transform[] spawnPoints;
    public WaveEnemy[] enemies;
}

[Serializable]
public struct WaveEnemy
{
    public GameObject enemy;

    [Tooltip("This is the time it takes in milliseconds for a tick to complete")]
    [Range(1f, 500f)]
    public float tickTime;
    [Tooltip("This is the rate that they will spawn per tick")]
    [Range(1f, 100f)]
    public float spawnPerTick;
    [Range(1, 100)]
    public int amount;
}
