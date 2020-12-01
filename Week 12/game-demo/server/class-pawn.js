const NetworkObject = require("./class-networkobject.js").NetworkObject;

exports.Pawn = class Pawn extends NetworkObject {
	constructor(){
		
		super();
		this.classID = "PAWN";
	}
	update(game){

		this.position.x = Math.sin(game.time);

	}
	serialize(){
		const b = super.serialize();

		///

		return b;
	}
	deserialize(){
		// TODO..
	}
}
