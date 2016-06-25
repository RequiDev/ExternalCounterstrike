using SharpDX.DirectWrite;

namespace ExternalCounterstrike.Overlay
{
    internal class LayoutBuffer
    {
        public string Text;
        public TextLayout TextLayout;

        public LayoutBuffer(string text, TextLayout layout)
        {
            Text = text;
            TextLayout = layout;
            TextLayout.TextAlignment = TextAlignment.Leading;
            TextLayout.WordWrapping = WordWrapping.NoWrap;
        }

        public void Dispose()
        {
            TextLayout.Dispose();
            Text = null;
        }
    }
}
