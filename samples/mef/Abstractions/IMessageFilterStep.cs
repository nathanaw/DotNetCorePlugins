namespace Plugin.Abstractions
{

    /// <summary>
    /// A filter step that takes a string, and returns a string.
    /// For use in the MessagePipeline, which is used in the controler actions to format a display message.
    /// </summary>
    /// <seealso cref="Plugin.Abstractions.IPipelineFilterStep{System.String}" />
    public interface IMessageFilterStep: IPipelineFilterStep<string>
    {
    }
}
