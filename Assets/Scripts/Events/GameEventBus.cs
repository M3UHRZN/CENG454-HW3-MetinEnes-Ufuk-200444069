using System;
using UnityEngine;

public static class GameEventBus
{
    public static event Action<float, float> OnCoreDamaged;
    public static event Action<int> OnEnemyDied;
    public static event Action<int> OnWaveCompleted;
    public static event Action<bool> OnGameOver;

    public static void RaiseCoreDamaged(float currentHP, float maxHP)
    {
        Debug.Log($"[GameEventBus] CoreDamaged → HP: {currentHP}/{maxHP}");
        OnCoreDamaged?.Invoke(currentHP, maxHP);
    }

    public static void RaiseEnemyDied(int killCount)
    {
        Debug.Log($"[GameEventBus] EnemyDied → Kill Count: {killCount}");
        OnEnemyDied?.Invoke(killCount);
    }

    public static void RaiseWaveCompleted(int waveIndex)
    {
        Debug.Log($"[GameEventBus] WaveCompleted → Wave: {waveIndex}");
        OnWaveCompleted?.Invoke(waveIndex);
    }

    public static void RaiseGameOver(bool playerWon)
    {
        Debug.Log($"[GameEventBus] GameOver → Player Won: {playerWon}");
        OnGameOver?.Invoke(playerWon);
    }
}
