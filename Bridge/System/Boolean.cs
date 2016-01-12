using Bridge;

namespace System
{
    [External]
    [Name("Boolean")]
    //[Constructor("!!")]
    public struct Boolean
    {
        [Template("!!")]
        public Boolean(object value)
        {
        }

        [Template( "Bridge.Boolean.tryParse({s}, {result})" )]
        public static bool TryParse( string s, out bool result )
        {
            result = false;
            return false;
        }
    }
}