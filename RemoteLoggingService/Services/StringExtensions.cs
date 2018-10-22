using System;

namespace RemoteLoggingService.Services
{
    public static class Extensions
    {
        public static bool CustomContains(this string str, string substring)
        {
            if (String.IsNullOrEmpty(substring))
            {
                return true;
            }
            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            return str.Contains(substring, comp);     
        }
        public static bool Contains(this String str, String substring,
                               StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring",
                                                "substring cannot be null.");
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison",
                                            "comp");

            return str.IndexOf(substring, comp) >= 0;
        }

    }    

}
