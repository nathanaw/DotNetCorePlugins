using Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace WebAppPlugin2.MessageFilters
{

    [Export(typeof(IMessageFilterStep))]
    [ExportMetadata("Context", "")]
    [ExportMetadata("Priority", 70)]
    public class LowerCaseVowelsFilterStep : IMessageFilterStep
    {

        private readonly string[] _vowels = new string[] { "A", "E", "I", "O", "U" };

        public string StepName => nameof(LowerCaseVowelsFilterStep);

        public string Execute(string value)
        {
            foreach (var letter in _vowels)
            {
                value = value.Replace(letter, letter.ToLowerInvariant());
            }

            return value;
        }

    }
}
