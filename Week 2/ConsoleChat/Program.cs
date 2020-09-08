using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleChat
{
    class Program
    {

        static TcpClient socket = new TcpClient();

        static void Main(string[] args)
        {

            //socket.BeginConnect("127.0.0.1", 320, new AsyncCallback(HandleConnection), null);

            /*
            new Thread(() => { }).Start();

            Task.Run(() => {
                socket.Connect("127.0.0.1", 320); // thread-blocking..
                Console.WriteLine("We are now connected to the server");
            });
            */

            // in a separate Task,
            // connect to the server
            // and listen for input:
            ConnectToServer();

            Console.WriteLine("Hello World!");
            //Console.WriteLine(DateTime.Now.Ticks);

            // client-side input loop:
            while (true)
            {
                string input = Console.ReadLine();

                byte[] data = Encoding.ASCII.GetBytes(input);

                socket.GetStream().Write(data, 0, data.Length);
            }
        }

        async static void ConnectToServer()
        {
            try
            {
                await socket.ConnectAsync("127.0.0.1", 320);
                Console.WriteLine("We are now connected to the server...");
                //Console.WriteLine(DateTime.Now.Ticks);
            } catch
            {
                Console.WriteLine("Could not connect to server...");
                return;
            }

            // get data from server:
            while (true)
            {
                byte[] data = new byte[socket.Available];

                await socket.GetStream().ReadAsync(data, 0, data.Length);

                Console.WriteLine(Encoding.ASCII.GetString(data));
            }

        }

        static void HandleConnection(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);
                Console.WriteLine("Now connected to the server...");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
        }
    }
}
