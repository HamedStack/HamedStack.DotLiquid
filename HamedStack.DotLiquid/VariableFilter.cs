namespace HamedStack.DotLiquid
{
    public static class VariableFilter
    {
        public static string SafeVar(string input, string tagName)
        {

            if (input == null)
            {
                return $"{DotLiquidUtility.Indicator}{tagName}{DotLiquidUtility.Indicator}";
            }
            return input;
        }

        public static string IgnoreSafeVar(string input)
        {
            return input;
        }
    }
}
