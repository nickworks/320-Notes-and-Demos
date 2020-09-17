const net = require("net");


// represents our protocol
const Packet = {
	charEndOfPacket:"\n",
	charDelimiter:"\t",
	buildChat:function(usernameFrom,message){
		return this.buildFromParts(["CHAT", usernameFrom, message]);
	},
	buildAnnouncement:function(message){
		return this.buildFromParts(["ANNC", message]);
	},
	buildNameOkay:function(){
		return this.buildFromParts(["NOKY"]);
	},
	buildNameBad:function(error){
		return this.buildFromParts(["NBAD",error]);
	},
	buildDM:function(usernameFrom,message){
		return this.buildFromParts(["DMSG",usernameFrom,message]);
	},
	buildList:function(arrOfClients){

		const arrayOfUsernames = [];

		arrOfClients.forEach(c=>{
			if(c.username) arrayOfUsernames.push(c.username);
			else arrayOfUsernames.push(c.socket.localAddress);
		});

		arrayOfUsernames.unshift("LIST");

		return this.buildFromParts(arrayOfUsernames);
	},
	buildFromParts:function(arr){
		return arr.join(this.charDelimiter)+this.charEndOfPacket;
	},
};

class Client {
	constructor(socket, server){

		this.buffer = "";
		this.username = "";
		this.socket = socket;
		this.server = server;
		this.socket.on("error", (e)=>this.onError(e));
		this.socket.on("close", ()=>this.onDisconnect());
		this.socket.on("data", (d)=>this.onData(d));
	}

	onError(errMsg){
		console.log("ERROR with "+this.socket.localAddress+" : "+errMsg);
	}
	onDisconnect(){
		server.onClientDisconnect(this);
	}
	onData(data){


		this.buffer += data;

		// split buffer apart into array of "packets":
		const packets = this.buffer.split("\n");

		// remove last item in array
		// and set the buffer to it:
		this.buffer = packets.pop();

		//console.log(packets.length + " new packets received from "+this.socket.localAddress);

		// handle ALL complete packets:
		packets.forEach(p=>this.handlePacket(p));
	}
	handlePacket(packet){

		// split the packet into parts:
		const parts = packet.split("\t");

		switch(parts[0]){
			case "CHAT":

				server.broadcast(Packet.buildChat(this.username, parts[1]));

				break;
			case "DMSG":
				break;
			case "NAME":

				const newname = parts[1];

				// TODO: accept or reject new name

				this.username = newname;
				this.sendPacket( Packet.buildNameOkay() );

				// TODO: send LIST packet to all users

				break;
			case "LIST":

				this.sendPacket( Packet.buildList( this.server.clients ));

				break;
		}
	}
	sendPacket(packet){
		this.socket.write(packet);
	}
}


class Server {
	constructor(){

		this.port = 320;

		this.clients = []; // currently connected clients

		this.socket = net.createServer({}, c=>this.onClientConnect(c));
		this.socket.on("error", e=>this.onError(e));
		this.socket.listen({port:this.port}, ()=>this.onStartListen());
	}
	onStartListen(){
		console.log("The server is now listening on port "+this.port);
	}
	onClientConnect(socketToClient){
		console.log("A new client connected from "+socketToClient.localAddress);

		const client = new Client(socketToClient, this);
		this.clients.push(client);
		//TODO: broadcast a LIST packet to everyone
	}
	onClientDisconnect(client){
		// remove this client object from the server list:
		this.clients.splice(this.clients.indexOf(client), 1);
		//TODO: broadcast a LIST packet to everyone
	}
	onError(errMsg){
		console.log("ERROR: " + errMsg);
	}
	broadcast(packet){

		// sends a packet to all
		this.clients.forEach(c=>{
			c.sendPacket(packet);
		});
	}
	
}

const server = new Server();