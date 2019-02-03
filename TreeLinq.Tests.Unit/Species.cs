using System;
using System.Linq;

namespace TreeLinq.Tests.Unit
{
    [Serializable]
    internal class Species
    {
        public Species()
        {
        }

        public Species(string name, params Species[] subspecies)
        {
            Name = name;
            Subspecies = subspecies;
        }

        public string Name { get; }

        public int Counter { get; set; }

        public Species[] Subspecies { get; } = new Species[0];

        public override int GetHashCode()
            => Name.GetHashCode();

        public override bool Equals(object obj)
            => obj is Species other && 
                Name == other.Name &&
                Subspecies.SequenceEqual(other.Subspecies);

        public override string ToString()
            => $"{base.ToString()}: {Name}";
    }
}
