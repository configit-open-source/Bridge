using Bridge.Html5;
using Bridge.QUnit;
using Bridge;

namespace ClientTestLibrary
{
    // Tests tBoolean functions
    internal class TestBooleanFunctions
    {
        // String functions
        public static void TryParse(Assert assert)
        {
            // In PhantomJS some correct tests failed. We will skip them in this environment.
            var isPhantomJs = Utilities.BrowserHelper.IsPhantomJs();

            bool v0 = true;
            assert.Equal(System.Boolean.TryParse( "False", out v0 ), true);
            assert.Equal(v0, false, "Boolean.TryParse 'False' gives false");

            bool v1 = true;
            assert.Equal(System.Boolean.TryParse( "false", out v1 ), true);
            assert.Equal(v1, false, "Boolean.TryParse 'false' gives false");

            bool v2 = true;
            assert.Equal(System.Boolean.TryParse( "True", out v2 ), true);
            assert.Equal(v2, true, "Boolean.TryParse 'True' gives true");

            bool v3 = true;
            assert.Equal(System.Boolean.TryParse( "true", out v3 ), true);
            assert.Equal(v3, true, "Boolean.TryParse 'true' gives true");

            bool v4 = true;
            assert.Equal(System.Boolean.TryParse( "Invalid", out v4 ), false);
            assert.Equal(v4, false, "Boolean.TryParse 'Invalid' gives false and fails");
          }
    }
}
