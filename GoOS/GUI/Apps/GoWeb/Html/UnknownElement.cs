#nullable enable
namespace GoOS.GUI.Apps.GoWeb.Html
{
    public class UnknownElement : Element
    {
        public UnknownElement(string tag)
        {
            Tag = tag;
        }

        public override string GetTag() => Tag;

        public string Tag { get; init; }
    }
}
