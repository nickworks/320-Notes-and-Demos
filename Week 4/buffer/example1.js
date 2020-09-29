

const buff1 = Buffer.from("hello");
const buff2 = Buffer.from([0, 255, 32]);
const buff3 = Buffer.alloc(10);

buff3.writeUInt8(255, 3);
buff3.writeUInt16BE(1024, 5); // Big-Endian notation writes left-to-right

var val = buff3.readUInt8(3);

console.log(val);

console.log(buff3);
