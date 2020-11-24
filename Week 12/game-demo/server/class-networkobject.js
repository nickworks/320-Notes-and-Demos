
// This is any object that can be sent over the network...

exports.NetworkObject = class NetworkObject {

	static _idCount = 0; // pls don't use

	constructor(){

		this.classID = "NWOB";
		this.networkID = ++NetworkObject._idCount;
	}

	serialize(){
		// TODO..
	}
	deserialize(){
		// TODO..
	}

}