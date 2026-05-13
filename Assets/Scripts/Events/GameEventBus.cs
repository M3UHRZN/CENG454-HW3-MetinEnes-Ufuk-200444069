using System;

public static class GameEventBus
{
    public static event Action<float, float> OnCoreDamaged;
    public static event Action<int> OnEnemyDied;
    public static event Action<int> OnWaveCompleted;
    public static event Action<bool> OnGameOver;

    public static void RaiseCoreDamaged(float currentHP, float maxHP) =>
        OnCoreDamaged?.Invoke(currentHP, maxHP);

    public static void RaiseEnemyDied(int killCount) =>
        OnEnemyDied?.Invoke(killCount);

    public static void RaiseWaveCompleted(int waveIndex) =>
        OnWaveCompleted?.Invoke(waveIndex);

    public static void RaiseGameOver(bool playerWon) =>
        OnGameOver?.Invoke(playerWon);
}
