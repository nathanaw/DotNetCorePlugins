using Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace WebAppPlugin1.MessageFilters
{

    [Export(typeof(IMessageFilterStep))]
    [ExportMetadata("Context", "About")]
    [ExportMetadata("Priority", 20)]
    public class SnakeCaseFilterStep : IMessageFilterStep
    {

        public string StepName => nameof(SnakeCaseFilterStep);

        public string Execute(string value)
        {
            return value?.Replace(' ', '-');
        }

    }
}
