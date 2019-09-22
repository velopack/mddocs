﻿using System;
using Grynwald.MarkdownGenerator;
using Grynwald.MdDocs.CommandLineHelp.Model;
using Grynwald.MdDocs.Common.Pages;

namespace Grynwald.MdDocs.CommandLineHelp.Pages
{
    /// <summary>
    /// Page that renders documentation for a application with multiple sub-commands.
    /// Shows
    /// <list type="bullet">
    ///     <item>Application name</item>
    ///     <item>Application version</item>
    ///     <item>Application usage (if the assembly has a AssemblyUsage attribute)</item>
    ///     <item>A table of sub-commands linking to the documentation for the command (see <see cref="CommandPage"/>).</item>
    /// </list>
    /// </summary>
    public class MultiCommandApplicationPage : IDocument
    {
        private readonly MultiCommandApplicationDocumentation m_Model;
        private readonly DocumentSet<IDocument> m_DocumentSet;
        private readonly ICommandLineHelpPathProvider m_PathProvider;


        public MultiCommandApplicationPage(DocumentSet<IDocument> documentSet, ICommandLineHelpPathProvider pathProvider, MultiCommandApplicationDocumentation model)
        {
            m_DocumentSet = documentSet ?? throw new ArgumentNullException(nameof(documentSet));
            m_PathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
            m_Model = model ?? throw new ArgumentNullException(nameof(model));
        }


        public void Save(string path) => GetDocument().Save(path);


        internal MdDocument GetDocument()
        {
            return new MdDocument()

                // Application name and version
                .Add(new MdHeading(1, $"{m_Model.Name} Command Line Reference"))
                .Add(new ApplicationVersionBlock(m_Model))

                // Usage (data from ApplicationUsage attribute)
                .AddIf(m_Model.Usage.Count > 0, new MdHeading(2, "Usage"))
                .AddIf(m_Model.Usage.Count > 0, new MdParagraph(String.Join(Environment.NewLine, m_Model.Usage)))

                // table of the applications sub-commands
                .AddIf(m_Model.Commands.Count > 0, new MdHeading(2, "Commands"))
                .AddIf(m_Model.Commands.Count > 0, GetCommandsTable())

                // footer
                .Add(new PageFooter());
        }


        private MdTable GetCommandsTable()
        {
            var table = new MdTable(new MdTableRow("Name", "Description"));
            foreach (var command in m_Model.Commands)
            {
                var commandPage = m_DocumentSet[m_PathProvider.GetPath(command)];

                var link = m_DocumentSet.GetLink(this, commandPage, command.Name);

                table.Add(new MdTableRow(link, command.HelpText ?? ""));
            }
            return table;
        }
    }
}