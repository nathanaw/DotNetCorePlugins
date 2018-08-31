using System.ComponentModel;

namespace Plugin.Abstractions
{

    /// <summary>
    /// A generic pipeline filter step base interface.
    /// </summary>
    /// <typeparam name="T">The input and output type.</typeparam>
    public interface IPipelineFilterStep<T>
    {

        string StepName { get; }

        T Execute(T value);

    }


    /// <summary>
    /// A Metadata class for use by MEF when importing Lazy imports.
    /// </summary>
    public interface IPipelineFilterStepMetadata
    {

        /// <summary>
        /// The name of the context (controller action) where this step is applicable.
        /// Use empty string to represent applicable for all actions.
        /// </summary>
        /// <remarks>
        /// While the use of MEF metadata is not strictly needed here, it shows how to use the feature.
        /// </remarks>
        [DefaultValue("")]
        string Context { get; }


        /// <summary>
        /// The relative priority of this step in the pipeline.
        /// Steps will be ordered numerically, ascending. (Priority 1 before priority 2, etc).
        /// </summary>
        [DefaultValue(50)]
        int Priority { get; }

    }


}
