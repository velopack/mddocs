﻿using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.ApiReference.Model;
using Microsoft.Extensions.Logging;

namespace Grynwald.MdDocs.ApiReference.Pages
{
    internal class MethodPage : OverloadableMemberPage<MethodDocumentation, MethodOverloadDocumentation>
    {
        public override OutputPath OutputPath { get; }


        public MethodPage(PageFactory pageFactory, string rootOutputPath, MethodDocumentation model, ILogger logger)
            : base(pageFactory, rootOutputPath, model, logger)
        {
            OutputPath = new OutputPath(GetTypeDir(Model.TypeDocumentation), "Methods", $"{Model.Name}.md");
        }


        protected override MdHeading GetPageHeading() =>
           new MdHeading($"{Model.TypeDocumentation.DisplayName}.{Model.Name} Method", 1);
    }
}
