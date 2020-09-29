const net = require("net");


const socket = net.connect({port:320, ip:"127.0.0.1"}, ()=>{
	console.log("connected to server...");

	const buff = Buffer.alloc(9);
	buff.write("JOIN");
	buff.writeUInt8(4, 4);
	buff.write("Nick", 5);

	socket.write(buff);
});

socket.on("error", e=>{
	console.log("ERROR: "+e);
});
