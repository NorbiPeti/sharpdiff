namespace SharpDiff
{
    public class FormatType
    {
        public FormatType(string format)
        {
            Name = format;
        }

        public string Name { get; private set; }
    }
}