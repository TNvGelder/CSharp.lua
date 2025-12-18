using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSystem
{
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison
    }

    public class DamageResult
    {
        public float RawDamage { get; set; }
        public float FinalDamage { get; set; }
        public bool IsCritical { get; set; }
        public bool IsDodged { get; set; }
        public DamageType Type { get; set; }

        public override string ToString()
        {
            if (IsDodged) return "DODGED!";
            string crit = IsCritical ? " (CRIT!)" : "";
            return $"{FinalDamage:F1} {Type} damage{crit}";
        }
    }

    public class CombatComponent : IComponent
    {
        public string Name => "Combat";

        private readonly Entity _owner;
        private readonly Random _random = new Random();

        public event Action<Entity, DamageResult> OnDamageDealt;
        public event Action<Entity, DamageResult> OnDamageTaken;
        public event Action OnDeath;

        public CombatComponent(Entity owner)
        {
            _owner = owner;
        }

        public DamageResult CalculateDamage(Entity target, float baseDamage, DamageType type)
        {
            var result = new DamageResult
            {
                RawDamage = baseDamage,
                Type = type
            };

            var attackerStats = _owner.GetComponent<StatsComponent>();
            var defenderStats = target.GetComponent<StatsComponent>();

            if (attackerStats == null || defenderStats == null)
            {
                result.FinalDamage = baseDamage;
                return result;
            }

            // Check for dodge
            float dodgeChance = defenderStats.GetStatValue("DodgeChance");
            if (_random.NextDouble() * 100 < dodgeChance)
            {
                result.IsDodged = true;
                result.FinalDamage = 0;
                return result;
            }

            // Check for crit
            float critChance = attackerStats.GetStatValue("CritChance");
            float critMultiplier = attackerStats.GetStatValue("CritMultiplier");
            if (critMultiplier == 0) critMultiplier = 150; // Default 150%

            if (_random.NextDouble() * 100 < critChance)
            {
                result.IsCritical = true;
                baseDamage *= critMultiplier / 100;
            }

            // Apply attack power
            float attackPower = attackerStats.GetStatValue("AttackPower");
            float damage = baseDamage + attackPower;

            // Apply defense
            float defense = defenderStats.GetStatValue("Defense");
            float damageReduction = defense / (defense + 100); // Diminishing returns formula
            damage *= (1 - damageReduction);

            // Apply resistance for elemental damage
            if (type != DamageType.Physical)
            {
                string resistStat = type.ToString() + "Resist";
                float resistance = defenderStats.GetStatValue(resistStat);
                damage *= (1 - resistance / 100);
            }

            result.FinalDamage = Math.Max(1, damage); // Minimum 1 damage
            return result;
        }

        public void Attack(Entity target, float baseDamage, DamageType type = DamageType.Physical)
        {
            var result = CalculateDamage(target, baseDamage, type);

            OnDamageDealt?.Invoke(target, result);

            var targetCombat = target.GetComponent<CombatComponent>();
            targetCombat?.TakeDamage(_owner, result);
        }

        public void TakeDamage(Entity attacker, DamageResult result)
        {
            if (result.IsDodged) return;

            OnDamageTaken?.Invoke(attacker, result);

            var stats = _owner.GetComponent<StatsComponent>();
            if (stats != null)
            {
                var healthStat = stats.GetStat("Health");
                if (healthStat != null)
                {
                    healthStat.BaseValue -= result.FinalDamage;
                    if (healthStat.Value <= 0)
                    {
                        _owner.IsActive = false;
                        OnDeath?.Invoke();
                    }
                }
            }
        }

        public void Update(float deltaTime)
        {
            // Combat component doesn't need regular updates
        }
    }
}
