﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MdDoc.Model
{
    public sealed class PropertyId : TypeMemberId, IEquatable<PropertyId>
    {
        public string Name { get; }
        
        public IReadOnlyList<TypeId> Parameters { get; }


        public PropertyId(TypeId definingType, string name) : this(definingType, name, Array.Empty<TypeId>())
        { }

        public PropertyId(TypeId definingType, string name, IReadOnlyList<TypeId> parameters) : base(definingType)
        {            
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Value must not be null or empty", nameof(name));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            
            Name = name;
            Parameters = parameters;
        }


        public override bool Equals(object obj) => Equals(obj as PropertyId);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = DefiningType.GetHashCode() * 397;
                hash ^= StringComparer.Ordinal.GetHashCode(Name);
                
                foreach (var parameter in Parameters)
                {
                    hash ^= parameter.GetHashCode();
                }

                return hash;
            }
        }

        public bool Equals(PropertyId other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other == null)
                return false;

            return DefiningType.Equals(other.DefiningType) &&
                StringComparer.Ordinal.Equals(Name, other.Name) &&
                Parameters.SequenceEqual(other.Parameters);
        }
    }
}
