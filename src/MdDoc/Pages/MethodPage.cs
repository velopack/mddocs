﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MdDoc.Model;
using Mono.Cecil;
using static Grynwald.MarkdownGenerator.FactoryMethods;

namespace MdDoc.Pages
{
    class MethodPage : MemberPage
    {
        private readonly MethodDocumentation m_Model;       


        protected override TypeReference DeclaringType => m_Model.Definitions.First().DeclaringType;

        protected override OutputPath OutputPath { get; }


        public MethodPage(PageFactory pageFactory, DocumentationContext context, PathProvider pathProvider, MethodDocumentation model) 
            : base(pageFactory, context, pathProvider)
        {
            m_Model = model ?? throw new ArgumentNullException(nameof(model));
            OutputPath = m_PathProvider.GetMethodOutputPath(model.Definitions.First());
        }


        public override void Save()
        {
            var document = Document(
                Heading($"{m_Model.Name}.{m_Model.Name} Method", 1)
            );

            AddDeclaringTypeSection(document.Root);            

            AddOverloadsSection(document.Root, m_Model.Definitions);

            AddDetailSections(document.Root, m_Model.Definitions);

            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
            document.Save(OutputPath);
        }
    }
}