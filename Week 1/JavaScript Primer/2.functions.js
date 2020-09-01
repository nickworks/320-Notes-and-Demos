
// in JS, functions are "first-class citizens", functions are objects

// we have anonymous functions:

const sayHello = function(){
	console.log("hello");
};

// we can pass functions into other functions:

function doFunction(f){
	f();
}

doFunction( function(){
	console.log("wow!");
} );

// ES6 added arrow function:

const square = n => n*n;

const mult = (a, b) => { return a*b; };

console.log( mult(12, 3) );

// a real scenario for anonymous functions:

var people = ["nick", "sarah", "billy", "sam"];

people.forEach( item=>{
	console.log(item);
} );

