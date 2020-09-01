
// there are several ways to declare
// variables in JS:

var example = "macaroni";
let age = 82;
const doILikePizza = true;

// can't change value of a const:
// doILikePizza = false;


// JavaScript is untyped. Variables don't have
// datatypes, and can switch what datatype they store.

macaroni = 16.8;

// declaring functions:

function doSomething(n){

	if(n == 1){
		return true;
	} else {
		return "banana";
	}
}

// arrays can be declared literally

var myArray = []; // empty array
var students = ["Vince", "Dominic", "Keegan", "Logan", "Andrew"];
students.push("Billy");

var stuff = [1, "cow", [], false, null];

// objects can be literal as well:
// literal objects use JSON

var myObj = {};

myObj = {
	age: 17,
	favoriteColor: "blue",
	isDead: false,
	favoriteBooks: ["Dune", "LOTR"],
};

stuff = [true, 42, {x:13, y:22}, null];
