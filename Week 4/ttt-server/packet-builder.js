
exports.PacketBuilder = {
	join(responseID){
		const packet = Buffer.alloc(5);
		
		packet.write("JOIN", 0);
		packet.writeUInt8(responseID, 4);

		return packet;
	},
	chat(){

	},
	update(game){
		const packet = Buffer.alloc(15);
		packet.write("UPDT", 0);
		packet.writeUInt8(game.whoseTurn, 4);
		packet.writeUInt8(game.whoHasWon, 5);

		let offset = 6;
		for(let y = 0; y < game.board.length; y++){
			for(let x = 0; x < game.board[y].length; x++){

				packet.writeUInt8(game.board[y][x], offset);
				offset++;

			}
		}

		return packet;
	},
};
