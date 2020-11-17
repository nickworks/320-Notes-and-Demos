exports.Game = class Game {
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

		const player = this.server.getPlayer(0); // return nth client

		if(player){
			this.ballPos.x += player.input.axisH * 1 * this.dt; // ball moves 1m/s
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

		this.server.sendPacketToAll(packet);
	}
}