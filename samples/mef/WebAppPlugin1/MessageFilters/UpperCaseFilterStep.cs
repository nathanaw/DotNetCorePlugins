using Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace WebAppPlugin1.MessageFilters
{

    [Export(typeof(IMessageFilterStep))]
    [ExportMetadata("Context", "")]
    [ExportMetadata("Priority", 10)]
    public class UpperCaseFilterStep : IMessageFilterStep
    {

        public string StepName => nameof(UpperCaseFilterStep);

        public string Execute(string value)
        {
            return value.ToUpperInvariant();
        }

    }
}
