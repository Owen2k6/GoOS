#nullable enable
namespace GoOS.GUI.Apps.GoWeb.Html;

public class UnknownElement : Element
{
    public UnknownElement(string tag)
    {
        Tag = tag;
    }

    public string Tag { get; init; }

    public override string GetTag()
    {
        return Tag;
    }
}