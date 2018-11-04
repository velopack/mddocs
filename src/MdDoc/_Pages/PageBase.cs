﻿using Grynwald.MarkdownGenerator;
using Mono.Cecil;
using System;
using System.Linq;

using static Grynwald.MarkdownGenerator.FactoryMethods;

namespace MdDoc
{
    abstract class PageBase : IPage
    {
        protected readonly DocumentationContext m_Context;
        protected readonly PathProvider m_PathProvider;        


        public abstract string Name { get; }

        protected abstract OutputPath OutputPath { get; }


        public PageBase(DocumentationContext context, PathProvider pathProvider)
        {
            m_Context = context ?? throw new ArgumentNullException(nameof(context));
            m_PathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        }


        public abstract void Save();


        protected virtual MdSpan GetTypeNameSpan(TypeReference type) => GetTypeNameSpan(type, false);

        protected virtual MdSpan GetTypeNameSpan(TypeReference type, bool noLink)
        {
            if (type.IsArray)
            {
                var elementTypeSpan = GetTypeNameSpan(type.GetElementType(), noLink);
                return new MdCompositeSpan(elementTypeSpan, $"[]");
            }

            if (type is GenericInstanceType genericType && genericType.HasGenericArguments)
            {
                var arguments = genericType.GenericArguments;

                var typeName = genericType.Name.Replace($"`{arguments.Count}", "");

                return CompositeSpan(
                    typeName,
                    "<",
                    arguments.Select(t => GetTypeNameSpan(t, noLink)).Join(", "),
                    ">"
                );
            }

            if (noLink || !m_Context.IsDocumentedItem(type))
            {
                return new MdTextSpan(type.Name);
            }
            else
            {
                var typeOutputPath = m_PathProvider.GetOutputPath(type);
                return new MdLinkSpan(
                    type.Name,
                    OutputPath.GetRelativePathTo(typeOutputPath)
                );
            }
        }
    }
}
