﻿using CommandLine;

namespace Grynwald.MdDocs.CommandLineHelp.TestData
{
    [Verb("command3")]
    public class Command3Options
    {
        [Option("option1")]
        public string Option1Property { get; set; }

        [Option('x')]
        public string Option2Property { get; set; }

        [Option('y', "option3")]
        public string Option3Property { get; set; }

        [Option("option4", HelpText = "Option 4 Help text", Hidden = true, Default = "DefaultValue")]
        public string Option4Property { get; set; }
    }
}
