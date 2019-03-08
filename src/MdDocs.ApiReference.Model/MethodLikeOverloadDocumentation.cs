﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grynwald.MdDocs.ApiReference.Model.XmlDocs;
using Mono.Cecil;

namespace Grynwald.MdDocs.ApiReference.Model
{
    public abstract class MethodLikeOverloadDocumentation : OverloadDocumentation
    {
        private readonly IXmlDocsProvider m_XmlDocsProvider;


        public override string Signature { get; }

        public override IReadOnlyList<ParameterDocumentation> Parameters { get; }

        public override string CSharpDefinition { get; }

        public override IReadOnlyList<TypeParameterDocumentation> TypeParameters { get; }

        public override TypeId Type { get; }

        public override bool IsObsolete { get; }

        public override string ObsoleteMessage { get; }

        internal MethodDefinition Definition { get; }


        internal MethodLikeOverloadDocumentation(
            MethodDefinition definition,
            IXmlDocsProvider xmlDocsProvider) : base(definition?.ToMemberId(), xmlDocsProvider)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            m_XmlDocsProvider = xmlDocsProvider ?? throw new ArgumentNullException(nameof(xmlDocsProvider));

            Parameters = definition.HasParameters
                ? definition.Parameters.Select(p => new ParameterDocumentation(this, p, xmlDocsProvider)).ToArray()
                : Array.Empty<ParameterDocumentation>();

            Signature = MethodFormatter.Instance.GetSignature(Definition);

            CSharpDefinition = CSharpDefinitionFormatter.GetDefinition(definition);

            TypeParameters = LoadTypeParameters();

            Type = definition.ReturnType.ToTypeId();

            IsObsolete = Definition.IsObsolete(out var obsoleteMessage);
            ObsoleteMessage = obsoleteMessage;
        }


        private IReadOnlyList<TypeParameterDocumentation> LoadTypeParameters()
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