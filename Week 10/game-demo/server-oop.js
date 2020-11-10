class Game {
	constructor(server){

		this.frame = 0;
		this.time = 0;
		this.dt = 16/1000;

		this.timeUntilNextStatePacket = 0;

		this.ballPos = {
			x: 0,
			y: 0,
			z: 0
		};

		this.server = server;
		this.update();

	}
	update(){

		this.time += this.dt;
		this.frame++;

		if(this.server.clients.length > 0){
			const c = this.server.clients[0];
			this.ballPos.x += c.input.h * 1 * this.dt; // ball moves 1m/s
		}

		if(this.timeUntilNextStatePacket > 0){
			// count down
			this.timeUntilNextStatePacket -= this.dt;
		} else {
			this.timeUntilNextStatePacket = .1; // send 10% packets (~ 1/6 frames)
			this.sendBallPos();
		}

		setTimeout(()=>this.update(), 16);
	}
	sendBallPos(){
		const packet = Buffer.alloc(20);
		packet.write("BALL", 0);
		packet.writeUInt32BE(this.frame, 4);
		packet.writeFloatBE(this.ballPos.x, 8);
		packet.writeFloatBE(this.ballPos.y, 12);
		packet.writeFloatBE(this.ballPos.z, 16);

		this.server.broadcastToConnectedClients(packet);
	}
}

class Server {
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

		//console.log("just received " + packetID);

		switch(packetID){
			case "JOIN":
				if(!this.doesClientExist(rinfo)) {

					rinfo.input = {};
					rinfo.input.h = 0;

					this.clients.push(rinfo);
				}
				break;
			case "INPT":
				if(msg.length < 5) return;
				const h = msg.readInt8(4);
				this.updateClientInput(rinfo, h);
				break;
		}

		//console.log("message received from "+rinfo.address+" : "+rinfo.port);
	}
	updateClientInput(rinfo, horizontalMovement){
		for(let i = 0; i < this.clients.length; i++){
			const c = this.clients[i];
			if(c.address == rinfo.address && c.port == rinfo.port){
				c.input.h = horizontalMovement;
			}
		}
	}
	doesClientExist(rinfo){

		let value = false;

		this.clients.forEach(c=>{
			if(c.address == rinfo.address && c.port == rinfo.port) value = true;
		});

		return value;
	}
	broadcastToConnectedClients(packet){

		this.clients.forEach(c=>{
			this.sock.send(packet, 0, packet.length, c.port, c.address, ()=>{});
		});
	}
}



new Server();
