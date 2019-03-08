﻿using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.ApiReference.Model;

using static Grynwald.MarkdownGenerator.FactoryMethods;

namespace Grynwald.MdDocs.ApiReference.Pages
{
    class IndexerPage : OverloadableMemberPage<IndexerDocumentation, IndexerOverloadDocumentation>
    {
        public override OutputPath OutputPath { get; }


        public IndexerPage(PageFactory pageFactory, string rootOutputPath, IndexerDocumentation model)
            : base(pageFactory, rootOutputPath, model)
        {
            OutputPath = new OutputPath(GetTypeDir(Model.TypeDocumentation), "Indexers", $"{Model.Name}.md");
        }

        protected override MdHeading GetHeading() =>
            Heading($"{Model.TypeDocumentation.DisplayName}.{Model.Name} Indexer", 1);

        protected override void AddParametersSubSection(MdContainerBlock block, IndexerOverloadDocumentation overload)
        {
            base.AddParametersSubSection(block, overload);
            AddValueSubSection(block, overload);
        }

        protected virtual void AddValueSubSection(MdContainerBlock block, IndexerOverloadDocumentation overload)
        {
            block.Add(Heading("Indexer Value", 3));
            block.Add(
                GetMdParagraph(overload.Type)
            );

            if (overload.Value != null)
            {
                block.Add(ConvertToBlock(overload.Value));
            }
        }


        //No "Returns" subsection for indexers (there is a "Value" section instead)
        protected override void AddReturnsSubSection(MdContainerBlock block, IndexerOverloadDocumentation overload)
        { }
    }
}