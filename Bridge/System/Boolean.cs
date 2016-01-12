using Bridge;

namespace System
{
    [External]
    [Name("Boolean")]
    //[Constructor("!!")]
    public struct Boolean : IComparable<Boolean>
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

        [Template( "{value} === {other} ? 0 : {this} && !{other} ? 1 : -1" )]
        public int CompareTo( Boolean other ) {
           return 0;
        }
    }
}