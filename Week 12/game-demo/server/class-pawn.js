const NetworkObject = require("./class-networkobject.js").NetworkObject;

exports.Pawn = class Pawn extends NetworkObject {
	constructor(){
		
		super();
		this.classID = "PAWN";
	}
}
