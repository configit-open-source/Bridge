/* global Bridge */

"use strict";

Bridge.define('ClientTestLibrary.TestBooleanFunctions', {
    statics: {
        tryParse: function (assert) {
            // In PhantomJS some correct tests failed. We will skip them in this environment.
            var isPhantomJs = Bridge.get(ClientTestLibrary.Utilities.BrowserHelper).isPhantomJs();

            var v0 = { v : true };
            assert.equal(Bridge.Boolean.tryParse("False", v0), true);
            assert.equal(v0.v, false, "Boolean.TryParse 'False' gives false");

            var v1 = { v : true };
            assert.equal(Bridge.Boolean.tryParse("false", v1), true);
            assert.equal(v1.v, false, "Boolean.TryParse 'false' gives false");

            var v2 = { v : true };
            assert.equal(Bridge.Boolean.tryParse("True", v2), true);
            assert.equal(v2.v, true, "Boolean.TryParse 'True' gives true");

            var v3 = { v : true };
            assert.equal(Bridge.Boolean.tryParse("true", v3), true);
            assert.equal(v3.v, true, "Boolean.TryParse 'true' gives true");

            var v4 = { v : true };
            assert.equal(Bridge.Boolean.tryParse("Invalid", v4), false);
            assert.equal(v4.v, false, "Boolean.TryParse 'Invalid' gives false and fails");
        }
    }
});



Bridge.init();