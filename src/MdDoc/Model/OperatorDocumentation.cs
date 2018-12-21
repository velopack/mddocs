﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grynwald.Utilities.Collections;
using Mono.Cecil;

namespace MdDoc.Model
{
    public class OperatorDocumentation : MemberDocumentation
    {
        private readonly IDictionary<MemberId, OperatorOverloadDocumentation> m_Overloads;


        public OperatorKind Kind { get; }

        public IReadOnlyCollection<OperatorOverloadDocumentation> Overloads { get; }       

        public OperatorDocumentation(TypeDocumentation typeDocumentation, IEnumerable<MethodDefinition> definitions) : base(typeDocumentation)
        {        
            if (definitions == null)
                throw new ArgumentNullException(nameof(definitions));

            m_Overloads = definitions
                .Select(d => new OperatorOverloadDocumentation(this, d))
                .ToDictionary(x => x.MemberId);

            Overloads = ReadOnlyCollectionAdapter.Create(m_Overloads.Values);

            OperatorKind? previousKind = null;
            foreach (var overload in Overloads)
            {                
                if(previousKind.HasValue && previousKind.Value != overload.OperatorKind)
                {
                    throw new ArgumentException("Cannot combine overloads of different operators");
                }
                previousKind = overload.OperatorKind;
                Kind = overload.OperatorKind;
            }            
        }

        public override IDocumentation TryGetDocumentation(MemberId id)
        {
            if(id is MethodId methodId && 
               methodId.DefiningType.Equals(TypeDocumentation.TypeId) &&
               methodId.GetOperatorKind() == Kind)
            {
                return m_Overloads.GetValueOrDefault(methodId);
            }
            else
            {
                return TypeDocumentation.TryGetDocumentation(id);
            }            
        }
    }
}
