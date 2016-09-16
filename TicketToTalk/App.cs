using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// App
	/// </summary>
	public class App : Application
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.App"/> class.
		/// </summary>
		public App()
		{
			var periodController = new PeriodController();
			periodController.initStockPeriods();

			var articleController = new ArticleController();
			articleController.getFaviconURL("http://tickettotalk.app/api/home");

			var nav = new NavigationPage(new Login());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			MainPage = nav;
		}

		/// <summary>
		/// Prints all tables.
		/// </summary>
		public void printAllTables()
		{
			Debug.WriteLine("PRINTING USER TABLE.");
			var userDB = new UserDB();
			foreach (User u in userDB.GetUsers())
			{
				Console.WriteLine(u);
			}
			userDB.close();

			Debug.WriteLine("PRINTING PEOPLE TABLE.");
			PersonDB personDB = new PersonDB();
			foreach (Person p in personDB.GetPersons())
			{
				Console.WriteLine(p);
			}
			personDB.close();

			Debug.WriteLine("PRINTING PERSON_USER TABLE.");
			PersonUserDB puDB = new PersonUserDB();
			foreach (PersonUser pu in puDB.GetRelations())
			{
				Console.WriteLine(pu);
			}
			puDB.close();

			Debug.WriteLine("PRINTING INSPIRATION TABLE.");
			InspirationDB insDB = new InspirationDB();
			foreach (Inspiration ins in insDB.GetInspirations())
			{
				Console.WriteLine(ins);
			}
			insDB.close();

			Debug.WriteLine("PRINTING AREA TABLE.");
			var areaDB = new AreaDB();
			foreach (Area a in areaDB.GetAreas())
			{
				Console.WriteLine(a);
			}
			areaDB.close();

			Debug.WriteLine("PRINTING TICKET TABLE.");
			var ticketDB = new TicketDB();
			foreach (Ticket t in ticketDB.GetTickets())
			{
				Console.WriteLine(t);
			}
			ticketDB.close();
		}

		/// <summary>
		/// Clears the database.
		/// </summary>
		public void clearDatabase()
		{
			Debug.WriteLine("CLEARING USER TABLE.");
			var userDB = new UserDB();
			userDB.clearTable();
			userDB.close();

			Debug.WriteLine("CLEARING PEOPLE TABLE.");
			PersonDB personDB = new PersonDB();
			personDB.clearTable();
			personDB.close();

			Debug.WriteLine("CLEARING PERSON_USER TABLE.");
			PersonUserDB puDB = new PersonUserDB();
			puDB.clearTable();
			puDB.close();

			Debug.WriteLine("CLEARING INSPIRATION TABLE.");
			InspirationDB insDB = new InspirationDB();

			insDB.close();

			Debug.WriteLine("CLEARING AREA TABLE.");
			var areaDB = new AreaDB();
			areaDB.clearTable();
			areaDB.close();

			Debug.WriteLine("CLEARING TICKET TABLE.");
			var ticketDB = new TicketDB();
			ticketDB.clearTable();
			ticketDB.close();
		}

		/// <summary>
		/// Sends the audio.
		/// </summary>
		public async void sendAudio()
		{
			var audioBytes = MediaController.readBytesFromFile("test.wav");
			Debug.WriteLine("Audio Bytes Length: " + audioBytes.Length);

			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["audio"] = audioBytes;

			NetworkController net = new NetworkController();
			await net.sendGenericPostRequest("test/receiveAudio", parameters);
		}
	}
}

