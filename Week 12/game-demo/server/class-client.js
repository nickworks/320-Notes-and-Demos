const Pawn = require("./class-pawn.js").Pawn;
const Game = require("./class-game.js").Game;

exports.Client = class Client {

	static TIMEOUT = 8;

	constructor(rinfo){
		this.rinfo = rinfo;

		this.input = {
			axisH:0,
			axisV:0,
		};

		this.pawn = null;
		this.timeOfLastPacket = Game.Singleton.time; // measured in seconds
	}
	spawnPawn(game){
		
		if(this.pawn) return; // if pawn exists, do nothing...

		this.pawn = new Pawn();
		game.spawnObject( this.pawn );

	}
	update(){

		const game = Game.Singleton;

		if(game.time > this.timeOfLastPacket + Client.TIMEOUT){
			game.server.disconnectClient(this);
		}
	}
	onPacket(packet, game){
		if(packet.length < 4) return; // ignore packet
		const packetID = packet.slice(0,4).toString();

		this.timeOfLastPacket = game.time;

		switch(packetID){

			// TODO: handle other kinds of packets...

			case "INPT":
				if(packet.length < 5) return;
				this.input.axisH = packet.readInt8(4);

				// send input to Pawn object:
				if(this.pawn) this.pawn.input = this.input;

				break;

			default:
				console.log("ERROR: packet type not recgonized");
		}
	}
}
