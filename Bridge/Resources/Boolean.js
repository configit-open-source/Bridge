// @source Boolean.js

var boolean = {
    tryParse: function (s, v) {
        v.v = false;
        if (s === "False" || s === "false") {
	          v.v = false;
            return true;
        }
        if (s === "True" || s === "true") {
          v.v = true;
	        return true;
        }
        return false;
    }
}


Bridge.Boolean = boolean;
