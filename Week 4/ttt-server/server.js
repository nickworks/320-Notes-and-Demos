
const Client = require("./client.js").Client;
const PacketBuilder = require("./packet-builder.js").PacketBuilder;

exports.Server = {
	port:320,
	clients:[],
	maxConnectedUsers:8,
	start(game){

		this.game = game;

		this.socket = require("net").createServer({}, c=>this.onClientConnect(c));
		this.socket.on("error", e=>this.onError(e));
		this.socket.listen({port:this.port},()=>this.onStartListen());
	},
	onClientConnect(socket){
		console.log("New connection from "+socket.localAddress);


		if(this.isServerFull()){ // server full:

			const packet = PacketBuilder.join(9); // name REJECTED, server full
			socket.end(packet); // end connection w/ this client (REJECTED!)

		} else { // server NOT full:

			// instantiate a new Client object:

			const client = new Client(socket, this);
			this.clients.push(client);
		}
	},
	onClientDisconnect(client){

		// players free up their "seats":
		if(this.game.clientX == client) this.game.clientX = null;
		if(this.game.clientY == client) this.game.clientY = null;

		// TODO: select a spectator to take leaving user's seat?

		const index = this.clients.indexOf(client); // find the object in the array
		if(index >= 0) this.clients.splice(index, 1); // remove the object from the array
	},
	onError(e){
		console.log("ERROR with listener: "+e);
	},
	onStartListen(){
		console.log("Server is now listening on port "+this.port);
	},
	isServerFull(){
		return (this.clients.length >= this.maxConnectedUsers);
	},
	// this function returns a response id
	generateResponseID(desiredUsername, client){

		if(desiredUsername.length <= 3) return 4; // username too short!
		if(desiredUsername.length > 12) return 5; // username too long!

		const regex1 = /^[a-zA-Z0-9\[\]]+$/; // literal regex in JavaScript
		if(!regex1.test(desiredUsername)) return 6; // uses invalid characters!

		let isUsernameTaken = false;
		this.clients.forEach(c=>{
			if(c == client) return;
			if(c.username == desiredUsername) isUsernameTaken = true;
		});
		if(isUsernameTaken) return 7;

		const regex2 = /(fuck|fvck|fuk|shit|5hit|damn|faggot)/i;
		if(regex2.test(desiredUsername)) return 8;


		if(this.game.clientX == client) return 1; // you are already client X!
		if(this.game.clientO == client) return 2; // you are already client O!

		if(!this.game.clientX) {
			this.game.clientX = client;
			return 1; // you are now client X!
		}
		if(!this.game.clientO) {
			this.game.clientO = client;
			return 2; // you are now client O!
		}

		return 3; // you are a spectator!
	},
	broadcastPacket(packet){
		this.clients.forEach(c=>{
			//if(c.username){
				c.sendPacket(packet);
			//}
		});
	},
};