using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSystem
{
    /// <summary>
    /// Base interface for all game components
    /// </summary>
    public interface IComponent
    {
        string Name { get; }
        void Update(float deltaTime);
    }

    /// <summary>
    /// Base entity class that can hold components
    /// </summary>
    public class Entity
    {
        private static int _nextId = 1;

        public int Id { get; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        private readonly List<IComponent> _components = new List<IComponent>();

        public Entity(string name)
        {
            Id = _nextId++;
            Name = name;
        }

        public void AddComponent(IComponent component)
        {
            _components.Add(component);
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            return _components.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetComponents<T>() where T : class, IComponent
        {
            return _components.OfType<T>();
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            return _components.Any(c => c is T);
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            foreach (var component in _components)
            {
                component.Update(deltaTime);
            }
        }

        public override string ToString()
        {
            return $"Entity[{Id}]: {Name} ({_components.Count} components)";
        }
    }
}
