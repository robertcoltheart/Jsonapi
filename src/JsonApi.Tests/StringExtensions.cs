namespace JsonApi.Tests
{
    public static class StringExtensions
    {
        public static string ToDoubleQuoted(this string value)
        {
            return value.Replace('\'', '\"');
        }
    }
}
