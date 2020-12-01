
const Game = require("./class-game.js").Game;
const Client = require("./class-client.js").Client;

exports.Server = class Server {
	constructor(){

		this.clients = [];

		// create socket:
		this.sock = require("dgram").createSocket("udp4");

		// setup event-listeners:
		this.sock.on("error", (e)=>this.onError(e));
		this.sock.on("listening", ()=>this.onStartListen());
		this.sock.on("message", (msg,rinfo)=>this.onPacket(msg,rinfo));

		this.game = new Game(this);

		// start listening:
		this.port = 320;
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
			c.onPacket(msg);
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

		this.showClientList();

		// send CREATE replication packets to client for every object:
		const packet = this.game.makeREPL(false);
		this.sendPacketToClient(packet, client); // TODO: needs ACK!!

		return client;
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
		this.sock.send(packet, 0, packet.length, client.rinfo.port, client.rinfo.address, ()=>{});
	}
}
