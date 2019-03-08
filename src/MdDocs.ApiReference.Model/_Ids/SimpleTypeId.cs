﻿using System;
using System.Collections.Generic;

namespace Grynwald.MdDocs.ApiReference.Model
{
    /// <summary>
    /// Identifies a non-generic type
    /// </summary>
    public sealed class SimpleTypeId : TypeId, IEquatable<SimpleTypeId>
    {
        private static readonly IReadOnlyDictionary<string, string> s_BuiltInTypes = new Dictionary<string, string>()
        {
            { "System.Boolean", "bool" },
            { "System.Byte", "byte" },
            { "System.SByte", "sbyte" },
            { "System.Char", "char" },
            { "System.Decimal", "decimal" },
            { "System.Double", "double" },
            { "System.Single", "float" },
            { "System.Int32", "int" },
            { "System.UInt32", "uint" },
            { "System.Int64", "long" },
            { "System.UInt64", "ulong" },
            { "System.Object", "object" },
            { "System.Int16", "short" },
            { "System.UInt16", "ushort" },
            { "System.String", "string" },
            { "System.Void", "void" }
        };


        public override string DisplayName =>
            s_BuiltInTypes.TryGetValue(NamespaceAndName, out var builtinName)
                ? builtinName
                : Name;


        public override bool IsVoid => Namespace.IsSystem && Name == "Void";

        public SimpleTypeId(string namespaceName, string name) : this(new NamespaceId(namespaceName), name)
        { }

        public SimpleTypeId(NamespaceId @namespace, string name) : base(@namespace, name)
        { }


        public override bool Equals(object obj) => Equals(obj as SimpleTypeId);

        public bool Equals(SimpleTypeId other) => other != null && Equals((TypeId)other);

        public override bool Equals(TypeId other) => other is SimpleTypeId && base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }
}