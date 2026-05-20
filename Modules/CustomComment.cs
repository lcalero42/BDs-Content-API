namespace DbsContentApi;

/// <summary>
/// A localized comment string for a given language code.
/// Used with <see cref="CustomCommentRegistry"/> to inject custom text into the in-game comment UI.
/// </summary>
public class CustomComment
{
    /// <summary>ISO language code (e.g. <c>en</c>, <c>de</c>, <c>pt-BR</c>).</summary>
    public string Language { get; set; }

    /// <summary>The localized comment text displayed in the comment UI.</summary>
    public string Comment { get; set; }

    /// <summary>
    /// Creates a localized comment entry.
    /// </summary>
    /// <param name="language">ISO language code (e.g. <c>en</c>, <c>de</c>).</param>
    /// <param name="comment">The comment text for that language.</param>
    public CustomComment(string language, string comment)
    {
        Language = language;
        Comment = comment;
    }
}
