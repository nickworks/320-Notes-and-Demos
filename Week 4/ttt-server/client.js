
const PacketBuilder = require("./packet-builder.js").PacketBuilder;


exports.Client = class Client {
	constructor(sock, server){
		
		this.socket = sock;
		this.server = server;
		this.username = "";

		this.buffer = Buffer.alloc(0);

		this.socket.on("error",(e)=>this.onError(e));
		this.socket.on("close",()=>this.onClose());
		this.socket.on("data",(d)=>this.onData(d));
	}
	onError(errMsg){
		console.log("ERROR with client: "+errMsg);
	}
	onClose(){
		this.server.onClientDisconnect(this);
	}
	onData(data){

		//console.log("packet received: " + data);

		// add new data to buffer:
		this.buffer = Buffer.concat([this.buffer, data]);

		// parse buffer for packets

		if(this.buffer.length < 4) return;  // not enough data to process

		const packetIdentifier = this.buffer.slice(0, 4).toString();

		switch(packetIdentifier){
			case "JOIN":
				if(this.buffer.length < 5) return; // not enough data to process
				
				const lengthOfUsername = this.buffer.readUInt8(4); // gets one byte (the 5th one)
				if(this.buffer.length < 5 + lengthOfUsername) return;  // not enough data to process
				const desiredUsername = this.buffer.slice(5, 5+lengthOfUsername).toString();

				// check username...
				let responseType = this.server.generateResponseID(desiredUsername, this);

				// consume data out of the buffer:
				this.buffer = this.buffer.slice(5 + lengthOfUsername);

				console.log("user wants to change name: '"+desiredUsername+"' ");

				// build and send packet:
				const packet = PacketBuilder.join(responseType);
				this.sendPacket(packet);

				const packet2 = PacketBuilder.update(this.server.game);
				this.sendPacket(packet2);

				break;
			case "CHAT": break;
			case "PLAY":
				if(this.buffer.length < 6) return; // not enough data in buffer...

				const x = this.buffer.readUInt8(4);
				const y = this.buffer.readUInt8(5);

				console.log("user wants to play at: "+x+" "+y);

				this.buffer = this.buffer.slice(6);

				this.server.game.playMove(this, x, y);

				break;
			default:
				// don't recognize the packet.... :(
				console.log("ERROR: packet identifier NOT recognized ("+packetIdentifier+")");
				this.buffer = Buffer.alloc(0); // empty the buffer
				break;
		}

		// process packets (and consume data from buffer)

	}
	sendPacket(packet){
		this.socket.write(packet);
	}

};