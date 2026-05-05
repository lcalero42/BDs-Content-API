namespace DbsContentApi.Modules;

public class CustomComment
{
    public string Language { get; set; } // e.g., "en", "de", "pt-BR"
    public string Comment { get; set; }

    public CustomComment(string language, string comment)
    {
        Language = language;
        Comment = comment;
    }
}
