namespace System
{
    public static class StringExtensions
    {
        // Taken from http://codahale.com/a-lesson-in-timing-attacks/
        public static bool SafeEquals(this string a, string b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            var result = 0;
            for (var i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }
    }
}
