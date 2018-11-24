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
    class MethodPage : MemberPage<MethodDocumentation>
    {
        public override OutputPath OutputPath { get; }
            

        protected override TypeReference DeclaringType => Model.Definitions.First().DeclaringType;
        
        protected override MethodDocumentation Model { get; }


        public MethodPage(PageFactory pageFactory, string rootOutputPath, MethodDocumentation model) 
            : base(pageFactory, rootOutputPath)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));   
            OutputPath = new OutputPath(Path.Combine(GetTypeDir(Model.TypeDocumentation), "methods", $"{Model.TypeDocumentation.Name}.{Model.Name}.md"));
        }


        public override void Save()
        {
            var document = Document(
                Heading($"{Model.Name}.{Model.Name} Method", 1)
            );

            AddDeclaringTypeSection(document.Root);            

            AddOverloadsSection(document.Root, Model.Definitions);

            AddDetailSections(document.Root, Model.Definitions);

            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
            document.Save(OutputPath);
        }
    }
}
