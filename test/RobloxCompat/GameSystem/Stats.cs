using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSystem
{
    /// <summary>
    /// Represents a single stat with base value and modifiers
    /// </summary>
    public class Stat
    {
        public string Name { get; }
        public float BaseValue { get; set; }

        private readonly List<StatModifier> _modifiers = new List<StatModifier>();

        public Stat(string name, float baseValue)
        {
            Name = name;
            BaseValue = baseValue;
        }

        public float Value
        {
            get
            {
                float flat = _modifiers.Where(m => m.Type == ModifierType.Flat).Sum(m => m.Value);
                float percent = _modifiers.Where(m => m.Type == ModifierType.Percent).Sum(m => m.Value);
                return (BaseValue + flat) * (1 + percent / 100);
            }
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            _modifiers.Remove(modifier);
        }

        public void ClearModifiers()
        {
            _modifiers.Clear();
        }
    }

    public enum ModifierType
    {
        Flat,
        Percent
    }

    public class StatModifier
    {
        public string Source { get; }
        public ModifierType Type { get; }
        public float Value { get; }

        public StatModifier(string source, ModifierType type, float value)
        {
            Source = source;
            Type = type;
            Value = value;
        }
    }

    /// <summary>
    /// Component that holds multiple stats for an entity
    /// </summary>
    public class StatsComponent : IComponent
    {
        public string Name => "Stats";

        private readonly Dictionary<string, Stat> _stats = new Dictionary<string, Stat>();

        public Stat GetStat(string name)
        {
            if (_stats.TryGetValue(name, out var stat))
                return stat;
            return null;
        }

        public Stat GetOrCreateStat(string name, float defaultValue = 0)
        {
            if (!_stats.TryGetValue(name, out var stat))
            {
                stat = new Stat(name, defaultValue);
                _stats[name] = stat;
            }
            return stat;
        }

        public float GetStatValue(string name)
        {
            var stat = GetStat(name);
            return stat?.Value ?? 0;
        }

        public IEnumerable<string> StatNames => _stats.Keys;

        public void Update(float deltaTime)
        {
            // Stats don't need regular updates
        }
    }
}
