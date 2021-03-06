namespace Bridge.Html5
{
    /// <summary>
    /// The HTMLParagraphElement interface provides special properties (beyond those of the regular HTMLElement object interface it inherits) for manipulating &lt;p&gt; elements.
    /// </summary>
    [External]
    [Name("HTMLParagraphElement")]
    public class ParagraphElement : Element<OptionElement>
    {
        [Template("document.createElement('p')")]
        public ParagraphElement()
        {
        }
    }
}
