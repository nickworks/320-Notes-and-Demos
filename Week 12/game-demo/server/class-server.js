
const Game = require("./class-game.js").Game;
const Client = require("./class-client.js").Client;
const Pawn = require("./class-Pawn.js").Pawn;

exports.Server = class Server {
	constructor(){

		this.port = 319; // server listens on
		this.serverName = "Nick's cool server!";
		this.clients = [];
		this.timeUntilNextBroadcast = 5;

		// create socket:
		this.sock = require("dgram").createSocket("udp4");

		// setup event-listeners:
		this.sock.on("error", (e)=>this.onError(e));
		this.sock.on("listening", ()=>this.onStartListen());
		this.sock.on("message", (msg,rinfo)=>this.onPacket(msg,rinfo));

		this.game = new Game(this);

		// start listening:
		this.sock.bind(this.port);
	}
	onError(e){
		console.log("ERROR: "+e);
	}
	onStartListen(){
		console.log("Server is listening on port "+this.port);
	}
	onPacket(msg, rinfo){

		if(msg.length < 4) return;
		const packetID = msg.slice(0,4).toString();

		const c = this.lookupClient(rinfo);
		if(c) {
			c.onPacket(msg, this.game);
		} else {
			if(packetID == "JOIN"){
				this.makeClient(rinfo);
			}
		}
		
	}
	getKeyFromRinfo(rinfo){
		return rinfo.address+":"+rinfo.port;
	}
	lookupClient(rinfo){
		const key = this.getKeyFromRinfo(rinfo);
		return this.clients[key];
	}
	makeClient(rinfo){

		const key = this.getKeyFromRinfo(rinfo);
		const client = new Client(rinfo);

		this.clients[key] = client;

		// send CREATE replication packets to client for every object:
		const packet = this.game.makeREPL(false);
		this.sendPacketToClient(packet, client); // TODO: needs ACK!!

		// depending on scene (and other conditions) spawn Pawn:
		client.spawnPawn(this.game);

		this.showClientList();

		const packet2 = Buffer.alloc(5);
		packet2.write("PAWN", 0);
		packet2.writeUInt8(client.pawn.networkID, 4);
		this.sendPacketToClient(packet2, client);

		return client;
	}
	disconnectClient(client){

		// send out DELETE packet:
		if(client.pawn) this.game.removeObject(client.pawn); 

		// remove CLIENT from list:
		const key = this.getKeyFromRinfo(client.rinfo);
		delete this.clients[key];

		this.showClientList();
	}
	showClientList(){
		console.log(" ======== "+Object.keys(this.clients).length+" clients connected ======= ");
		for(var key in this.clients){
			console.log(key);
		}
	}
	getPlayer(num=0){

		num = parseInt(num);
		let i = 0;
		for(var key in this.clients){
			if(num == i) return this.clients[key];
			i++;
		}
	}
	sendPacketToAll(packet){
		for(var key in this.clients){
			this.sendPacketToClient(packet, this.clients[key]);
		}
	}
	sendPacketToClient(packet, client){
		console.log(client.rinfo);
		this.sock.send(packet, 0, packet.length, 321, client.rinfo.address, ()=>{});
	}
	broadcastPacket(packet){

		const clientListenPort = 321;

		this.sock.send(packet, 0, packet.length, clientListenPort, undefined);
	}
	broadcastServerHost(){

		const nameLength = this.serverName.length;
		const packet = Buffer.alloc(7+nameLength);

		packet.write("HOST", 0);
		packet.writeUInt16BE(this.port, 4);
		packet.writeUInt8(nameLength, 6);
		packet.write(this.serverName, 7);

		//const addr = this.sock.address();
		//console.log(addr);

		this.broadcastPacket(packet);
		//console.log("broadcast packet...");
	}
	update(game){
		// check clients for disconnects, etc.
		for(let key in this.clients){
			this.clients[key].update(game);
		}

		this.timeUntilNextBroadcast -= game.dt;
		if(this.timeUntilNextBroadcast <= 0){
			this.timeUntilNextBroadcast = 1.5;

			this.broadcastServerHost( );
		}

	}
}
