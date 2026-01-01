using Roblox;
using System;

namespace TestGame;

/// <summary>
/// Manages player currency with passive income.
/// Provides 10 coins every 5 seconds automatically.
/// </summary>
public class CurrencyManager
{
    private int _coins;
    private double _timeSinceLastIncome;
    private readonly double _incomeInterval = 5.0;  // seconds
    private readonly int _incomeAmount = 10;        // coins per interval

    /// <summary>
    /// Event fired when coin count changes.
    /// </summary>
    public event Action<int>? OnCoinsChanged;

    /// <summary>
    /// Current coin count.
    /// </summary>
    public int Coins => _coins;

    /// <summary>
    /// Creates a new currency manager with the specified starting coins.
    /// </summary>
    public CurrencyManager(int startingCoins = 10000)
    {
        _coins = startingCoins;
        _timeSinceLastIncome = 0;
    }

    /// <summary>
    /// Starts the passive income system using RunService.Heartbeat.
    /// Call this once after creating the manager.
    /// </summary>
    public void StartPassiveIncome()
    {
        var runService = Globals.game.GetService<RunService>();
        runService.Heartbeat.Connect(Update);
    }

    /// <summary>
    /// Called every frame to update passive income timer.
    /// </summary>
    private void Update(double deltaTime)
    {
        _timeSinceLastIncome += deltaTime;

        if (_timeSinceLastIncome >= _incomeInterval)
        {
            AddCoins(_incomeAmount);
            _timeSinceLastIncome = 0;
        }
    }

    /// <summary>
    /// Adds coins to the player's balance.
    /// </summary>
    public void AddCoins(int amount)
    {
        _coins += amount;
        OnCoinsChanged?.Invoke(_coins);
    }

    /// <summary>
    /// Checks if the player can afford a purchase.
    /// </summary>
    public bool CanAfford(int amount)
    {
        return _coins >= amount;
    }

    /// <summary>
    /// Attempts to spend coins. Returns true if successful.
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (_coins >= amount)
        {
            _coins -= amount;
            OnCoinsChanged?.Invoke(_coins);
            return true;
        }
        return false;
    }
}
