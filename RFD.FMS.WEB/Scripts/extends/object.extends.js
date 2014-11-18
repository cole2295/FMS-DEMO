Object.prototype.equals = function(obj) {
	if (this == obj) return true;
	if (typeof (obj) == "undefined" || obj == null || typeof (obj) != "object")
		return false;
	var thislength = 0;
	var thatlength = 0;
	for (var p in this) {
		thislength++;
	}
	for (var p in obj) {
		thatlength++;
	}
	if (thislength != thatlength)
		return false;
	if (obj.constructor == this.constructor) {
		for (var p in this) {
			if (typeof (this[p]) == "object") {
				if (!this[p].equals(obj[p]))
					return false;
			}
			else if (typeof (this[p]) == "function") {
				if (!this[p].toString().equals(obj[p].toString()))
					return false;
			}
			else if (this[p] != obj[p])
				return false;
		}
		return true;
	}
	return false;
}; 

