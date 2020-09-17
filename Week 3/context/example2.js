
// sometimes `this` is mapped onto other objects
// specifically, when using event-listeners

// `this` gets mapped as the event object (not global)
setTimeout(function(){ console.log(this) }, 100);

// arrow functions do NOT change the context of `this`:

setTimeout(()=>{ console.log(this) }, 100);


const net = require('net');

class Server {
	constructor(){

		this.port = 1234;

		const sock = net.createServer({}, ()=>{
			console.log(this.port);
		});
		/*
		// this doesn't work:
		const sock = net.createServer({}, function(){
			console.log(this.port);
		});
		*/

	}
}

