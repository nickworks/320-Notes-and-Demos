const net = require("net"); // import nodejs TCP socket module

const clients = []; // an array of all currently-connected clients

const listeningSocket = net.createServer({}, (socketToClient)=>{
	console.log(socketToClient.localAddress + " has connected!");

	clients.push(socketToClient); // add new client to list of clients

	socketToClient.on("error", (errMsg)=>{
		console.log("ERROR: " + errMsg);
	});
	socketToClient.on("close", ()=>{
		console.log("A client has disconnected...");
		const index = clients.indexOf(socketToClient); // index of this client
		clients.splice(index, 1); // remove client from array of clients
		//console.log(clients.length);
	});
	socketToClient.on("data", txt=>{
		console.log("client says: " + txt);
		BroadcastToAll(txt);
	});
	socketToClient.write("Welcome to my server. Be nice or GTFO");
});

listeningSocket.listen({port:320}, ()=>{
	console.log("The server is now listening for incoming connections...");

});

function BroadcastToAll(txt, clientToSkip){
	clients.forEach(client=>{
		if(client != clientToSkip) client.write(txt);
	});
}