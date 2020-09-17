
// how does the `this` keyword work?

// in oop, `this` refers to the object containing the code

const cat = "meow"; // global variable

function doSomething(){
	console.log(cat);
}

//doSomething();

function example1(){
	console.log(this);
}
class Example2 {
	constructor(){
		console.log(this);
	}
}

new Example2();
