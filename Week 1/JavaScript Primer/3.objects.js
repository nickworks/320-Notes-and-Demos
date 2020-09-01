
const obj = {};


// we can easily add properties

obj.x = 57;
obj.update = function(){
	console.log("wow, I'm updating");
};

obj.update();

// old js doesn't have classes

function Person(){
	this.name = "Jimmy";
	this.sayHello = ()=>{
		console.log("Hello, I'm " + this.name);
	};
}

var jim = new Person();

jim.sayHello();

// ES6 introduced classes to JS


class Sprite {

	constructor(){
		// properties:

		this.x = 0;
		this.y = 154;
		this.rotation = 45;
		this.isDead = false;
	}
	die(){
		this.isDead = true;
	}
}

class Enemy extends Sprite {
	constructor(){
		super();
	}
	spin(amount){
		this.rotation += amount;
	}
}

var e = new Enemy();

console.log(e.rotation);
e.spin(60);
console.log(e.rotation);

