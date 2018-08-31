using Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace MefWebApp.Pipelines
{

    /// <summary>
    /// A pipeline for running arbitrary steps on a string.
    /// Pluggable via MEF and dynamically loaded modules.
    /// </summary>
    [Export]
    public class MessagePipeline
    {

        /// <summary>
        /// The list of steps, as provided by MEF.
        /// The use of Lazy<> lets the code inspect metadata before instantiating the module.
        /// The use of IEnumerable<> allows MEF to supply multiple steps.
        /// </summary>
        private IEnumerable<Lazy<IMessageFilterStep, IPipelineFilterStepMetadata>> _steps;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePipeline"/> class.
        /// Relies on MEF to supply dependencies.
        /// </summary>
        /// <param name="steps">The steps, as supplied by MEF.</param>
        [ImportingConstructor]
        public MessagePipeline([ImportMany] IEnumerable<Lazy<IMessageFilterStep, IPipelineFilterStepMetadata>> steps)
        {
            _steps = steps;
        }

        //[ImportMany]
        //public IEnumerable<Lazy<IMessageFilterStep, IPipelineFilterStepMetadata>> Steps {
        //    get { return _steps; }
        //    set { _steps = value; }
        //}


        /// <summary>
        /// Executes the sequence of message pipeline filter steps.
        /// </summary>
        /// <param name="message">The message value to filter.</param>
        /// <param name="context">The context (name of controller action).</param>
        /// <returns></returns>
        public string Execute(string message, string context)
        {
            var stepsToExecute = GetSteps(context);

            // Run the steps
            var result = message;
            foreach (var step in stepsToExecute)
            {
                result = step.Execute(result);
            }
            return result;
        }


        /// <summary>
        /// Gets the steps that should execute for this context.
        /// </summary>
        /// <param name="context">The context name.</param>
        /// <returns>The list of steps to execute.</returns>
        public IList<IMessageFilterStep> GetSteps(string context)
        {
            // Select the steps to execute based on the context.
            return _steps.Where(s =>
                            {
                                return (
                                    string.IsNullOrEmpty(context)
                                    ||
                                    string.IsNullOrEmpty(s.Metadata.Context)
                                    ||
                                    context.Equals(s.Metadata.Context, StringComparison.OrdinalIgnoreCase)
                                );
                            })
                        .OrderBy(s => s.Metadata.Priority)
                        .Select(s => s.Value)
                        .ToList();
        }
    }

}
