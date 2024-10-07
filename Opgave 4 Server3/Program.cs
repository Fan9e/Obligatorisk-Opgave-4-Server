

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

class TcpServer
{
	static void Main()
	{
		TcpListener listener = new TcpListener(IPAddress.Any, 8888);//opretter en TCP-lytter på alle nætvrkadresser
		listener.Start();//strter servern så den begnder at lytte
		Console.WriteLine("Server started...");

		while (true) //Starter lykke og pga (true) køre den evigt
		{
			TcpClient client = listener.AcceptTcpClient();//den skal acceptere forbindelsen og retunere TCPClient
			Task.Run(() => HandleClient(client)); // Concurrent== kan håndtere flere klienter afgangen
		}
	}

	static void HandleClient(object obj)
	{
		TcpClient client = (TcpClient)obj; //Modtager TCPClient objekt fra metoden AcceptTcpClient
		NetworkStream ns = client.GetStream();//henter nætværkstream fra klienten
		StreamReader reader = new StreamReader(ns);//opretter en reader til at læse teskten fra ætnærkstream
		StreamWriter writer = new StreamWriter(ns) { AutoFlush = true };//opretter en wtriter til at skrive gennem nætværkstream
        //sørger for at sende data med det samme(AutoFlush = true)

        try//starter en try/chatch
		{
			string command = "";//initalisere en tom strengtil at gemme commands fra klienten 
			while ((command = reader.ReadLine()) != null && command.ToLower() != "stop")//løkken fortsætter en til man skrive "stop"
			{
				Console.WriteLine($"Command received: {command}");//udskriv den "command" på serverens side der er blevet modtaget

				// Send besked til klienten for at bede om tal
				writer.WriteLine("Input numbers");

				// Modtag tal fra klienten
				string[] numbers = reader.ReadLine().Split(' ');//læs klientens input
				int num1 = int.Parse(numbers[0]);//konventer num1= modtaget strenge til int=heltal
				int num2 = int.Parse(numbers[1]);//samme her med num2

				string result = "";//initalisere en tom streng til at håndtere/gemme resultat

				// Behandle kommando og udregner resultatet iforhold til hvad der er blevet valgt
				switch (command.ToLower())
				{
                    // Hvis kommandoen er "random", generer et tilfældigt tal mellem num1 og num2
                    case "random":
						Random random = new Random();
						result = random.Next(num1, num2 + 1).ToString();
						break; //hvis manhar valgt denne skal loopet stoppes her

                     // Hvis kommandoen er "add", lægges num1 og num2 sammen
                    case "add":
						result = (num1 + num2).ToString();
						break;//hvis manhar valgt denne skal loopet stoppes her

                    // Hvis kommandoen er "subtract", trækkes num2 fra num1
                    case "subtract":
						result = (num1 - num2).ToString();
						break;//hvis manhar valgt denne skal loopet stoppes her

                     // Hvis kommandoen ikke er genkendt, returneres en fejlmeddelelse
                    default:
						result = "Invalid command";
						break;//hvis manhar valgt denne skal loopet stoppes her
                }

				writer.WriteLine(result);// Send resultat tilbage til klienten
                Console.WriteLine($"Result sent: {result}"); //Udskriv resultatet
			}

			Console.WriteLine("Client disconnected or stopped.");//informere kliententet om at forbindelsen er slut
		}
		catch (Exception e) //hvs der sker en fejl laves en exception
		{
			Console.WriteLine($"Error: {e.Message}");//error besked bliver sendt
		}
		finally
		{
			client.Close(); //og til sidst bliver forindelsen lukket vis der sker en fejl eller handlingerne er slut
		}
	}
}

