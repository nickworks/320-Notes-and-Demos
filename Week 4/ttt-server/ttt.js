const Server = require("./server.js").Server;
const PacketBuilder = require("./packet-builder.js").PacketBuilder;

const Game = {
	whoseTurn:1,
	whoHasWon:0,
	board:[
		[0,0,0],
		[0,0,0],
		[0,0,0],
	],

	clientX:null, // player 1
	clientO:null, // player 2

	playMove(client, x, y){
		
		// ignore MOVEs after game has ended:
		if(this.whoHasWon > 0) return;

		// ignore everyone but clientX on clientX's turn:
		if(this.whoseTurn == 1 && client != this.clientX) return;

		// ignore everyone but clientO on clientO's turn:
		if(this.whoseTurn == 2 && client != this.clientO) return;

		if(x < 0) return; // ignore MOVEs off the board
		if(y < 0) return; // ignore MOVEs off the board

		if(y >= this.board.length) return; // ignore MOVEs off the board
		if(x >= this.board[y].length) return; // ignore MOVEs off the board

		if(this.board[y][x] > 0) return; // ignore MOVEs on taken spaces

		this.board[y][x] = this.whoseTurn; // sets board state

		this.whoseTurn = (this.whoseTurn == 1) ? 2 : 1; // toggles turn to next player

		this.checkStateAndUpdate();
	},
	checkStateAndUpdate(){
		
		// TODO: look for game over
		
		const packet = PacketBuilder.update(this);
		Server.broadcastPacket(packet);
	}
};

Server.start(Game);
