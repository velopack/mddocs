﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grynwald.Utilities.Collections;
using Grynwald.MdDocs.ApiReference.Model.XmlDocs;
using Mono.Cecil;

namespace Grynwald.MdDocs.ApiReference.Model
{
    public class TypeDocumentation : IDocumentation, IObsoleteableDocumentation
    {
        private readonly IXmlDocsProvider m_XmlDocsProvider;
        private readonly IDictionary<MemberId, FieldDocumentation> m_Fields;
        private readonly IDictionary<MemberId, EventDocumentation> m_Events;
        private readonly IDictionary<MemberId, PropertyDocumentation> m_Properties;
        private readonly IDictionary<string, IndexerDocumentation> m_Indexers;
        private readonly IDictionary<string, MethodDocumentation> m_Methods;
        private readonly IDictionary<OperatorKind, OperatorDocumentation> m_Operators;


        public MemberId MemberId => TypeId;

        public TypeId TypeId { get; }

        public ModuleDocumentation ModuleDocumentation { get; }

        public NamespaceDocumentation NamespaceDocumentation { get; }

        public string DisplayName => TypeId.DisplayName;

        public string AssemblyName => Definition.Module.Assembly.Name.Name;

        public TypeKind Kind { get; }

        public IReadOnlyCollection<FieldDocumentation> Fields { get; }

        public IReadOnlyCollection<EventDocumentation> Events { get; }

        public IReadOnlyCollection<PropertyDocumentation> Properties { get; }

        public IReadOnlyCollection<IndexerDocumentation> Indexers { get; }

        public ConstructorDocumentation Constructors { get; }

        public IReadOnlyCollection<MethodDocumentation> Methods { get; }

        public IReadOnlyCollection<OperatorDocumentation> Operators { get; }

        public IReadOnlyCollection<TypeId> InheritanceHierarchy { get; }

        public IReadOnlyCollection<TypeId> ImplementedInterfaces { get; }

        public IReadOnlyCollection<TypeId> Attributes { get; }

        public TextBlock Summary { get; }

        public TextBlock Remarks { get; }

        public IReadOnlyCollection<SeeAlsoElement> SeeAlso { get; }

        public IReadOnlyCollection<TypeParameterDocumentation> TypeParameters { get; }

        public string CSharpDefinition { get; }

        internal TypeDefinition Definition { get; }

        public TextBlock Example { get; }

        public bool IsObsolete { get; }

        public string ObsoleteMessage { get; }


        internal TypeDocumentation(
            ModuleDocumentation moduleDocumentation,
            NamespaceDocumentation namespaceDocumentation,
            TypeDefinition definition,
            IXmlDocsProvider xmlDocsProvider)
        {
            TypeId = definition.ToTypeId();

            ModuleDocumentation = moduleDocumentation ?? throw new ArgumentNullException(nameof(moduleDocumentation));
            NamespaceDocumentation = namespaceDocumentation ?? throw new ArgumentNullException(nameof(namespaceDocumentation));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            m_XmlDocsProvider = xmlDocsProvider ?? throw new ArgumentNullException(nameof(xmlDocsProvider));

            Kind = definition.Kind();

            m_Fields = definition.Fields
                .Where(field => field.IsPublic && !field.Attributes.HasFlag(FieldAttributes.SpecialName))
                .Select(field => new FieldDocumentation(this, field, xmlDocsProvider))
                .ToDictionary(f => f.MemberId);

            Fields = ReadOnlyCollectionAdapter.Create(m_Fields.Values);

            m_Events = definition.Events
                .Where(ev => (ev.AddMethod?.IsPublic == true || ev.RemoveMethod?.IsPublic == true))
                .Select(e => new EventDocumentation(this, e, xmlDocsProvider))
                .ToDictionary(e => e.MemberId);

            Events = ReadOnlyCollectionAdapter.Create(m_Events.Values);

            m_Properties = definition.Properties
                .Where(property => (property.GetMethod?.IsPublic == true || property.SetMethod?.IsPublic == true) && !property.HasParameters)
                .Select(p => new PropertyDocumentation(this, p, xmlDocsProvider))
                .ToDictionary(p => p.MemberId);

            Properties = ReadOnlyCollectionAdapter.Create(m_Properties.Values);

            m_Indexers = definition.Properties
                .Where(property => (property.GetMethod?.IsPublic == true || property.SetMethod?.IsPublic == true) && property.HasParameters)
                .GroupBy(p => p.Name)
                .Select(group => new IndexerDocumentation(this, group, xmlDocsProvider))
                .ToDictionary(indexer => indexer.Name);

            Indexers = ReadOnlyCollectionAdapter.Create(m_Indexers.Values);

            var ctors = definition.GetDocumentedConstrutors();
            if (ctors.Any())
                Constructors = new ConstructorDocumentation(this, ctors, xmlDocsProvider);

            m_Methods = definition.GetDocumentedMethods()
                .Where(m => !m.IsOperator())
                .GroupBy(x => x.Name)
                .Select(group => new MethodDocumentation(this, group, xmlDocsProvider))
                .ToDictionary(m => m.Name);

            Methods = ReadOnlyCollectionAdapter.Create(m_Methods.Values);

            m_Operators = definition.GetDocumentedMethods()
               .GroupBy(x => x.GetOperatorKind())
               .Where(group => group.Key.HasValue)
               .Select(group => new OperatorDocumentation(this, group, xmlDocsProvider))
               .ToDictionary(x => x.Kind);

            Operators = ReadOnlyCollectionAdapter.Create(m_Operators.Values);

            InheritanceHierarchy = LoadInheritanceHierarchy();

            Attributes = Definition
                .GetCustomAttributes()
                .Select(x => x.AttributeType.ToTypeId())
                .ToArray();

            ImplementedInterfaces = LoadImplementedInterfaces();

            TypeParameters = LoadTypeParameters();

            var documentationComments = m_XmlDocsProvider.TryGetDocumentationComments(MemberId);
            Summary = documentationComments?.Summary;
            Remarks = documentationComments?.Remarks;
            SeeAlso = documentationComments?.SeeAlso?.ToReadOnly() ?? Array.Empty<SeeAlsoElement>();
            Example = documentationComments?.Example;

            CSharpDefinition = CSharpDefinitionFormatter.GetDefinition(definition);

            IsObsolete = definition.IsObsolete(out var obsoleteMessage);
            ObsoleteMessage = obsoleteMessage;
        }


        public IDocumentation TryGetDocumentation(MemberId id)
        {
            switch (id)
            {
                case TypeId typeId when typeId.Equals(TypeId):
                    return this;

                case FieldId fieldId when fieldId.DefiningType.Equals(TypeId):
                    return m_Fields.GetValueOrDefault(fieldId);

                case EventId eventId when eventId.DefiningType.Equals(TypeId):
                    return m_Events.GetValueOrDefault(eventId);

                case PropertyId propertyId when propertyId.DefiningType.Equals(TypeId) && propertyId.Parameters.Count == 0:
                    return m_Properties.GetValueOrDefault(propertyId);

                case PropertyId propertyId when propertyId.DefiningType.Equals(TypeId) && propertyId.Parameters.Count > 0:
                    return m_Indexers.GetValueOrDefault(propertyId.Name);

                case MethodId methodId when methodId.DefiningType.Equals(TypeId):
                    if (methodId.IsConstructor())
                    {
                        return Constructors.TryGetDocumentation(methodId);
                    }

                    if (m_Methods.ContainsKey(methodId.Name))
                    {
                        return m_Methods[methodId.Name].TryGetDocumentation(methodId);
                    }

                    var operatorKind = methodId.GetOperatorKind();
                    if (operatorKind.HasValue && m_Operators.ContainsKey(operatorKind.Value))
                    {
                        return m_Operators[operatorKind.Value].TryGetDocumentation(methodId);
                    }

                    return null;

                default:
                    return ModuleDocumentation.TryGetDocumentation(id);
            }
        }


        private IReadOnlyCollection<TypeId> LoadInheritanceHierarchy()
        {
            if (Kind == TypeKind.Interface)
                return Array.Empty<TypeId>();

            var inheritance = new LinkedList<TypeId>();
            inheritance.AddFirst(TypeId);

            var currentBaseType = Definition.BaseType.Resolve();
            while (currentBaseType != null)
            {
                inheritance.AddFirst(currentBaseType.ToTypeId());
                currentBaseType = currentBaseType.BaseType?.Resolve();
            }

            return inheritance;
        }

        private IReadOnlyCollection<TypeId> LoadImplementedInterfaces()
        {
            if (!Definition.HasInterfaces)
                return Array.Empty<TypeId>();
            else
                return Definition.Interfaces.Select(x => x.InterfaceType.ToTypeId()).ToArray();
        }

        private IReadOnlyCollection<TypeParameterDocumentation> LoadTypeParameters()
        {
            if (!Definition.HasGenericParameters)
                return Array.Empty<TypeParameterDocumentation>();
            else
                return Definition.GenericParameters
                    .Select(p => new TypeParameterDocumentation(this, MemberId, p, m_XmlDocsProvider))
                    .ToArray();

        }
    }
}