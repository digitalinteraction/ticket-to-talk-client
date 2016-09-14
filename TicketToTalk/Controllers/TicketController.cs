using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Ticket controller.
	/// </summary>
	public class TicketController
	{
		TicketDB ticketDB = new TicketDB();
		NetworkController networkController = new NetworkController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketController"/> class.
		/// </summary>
		public TicketController()
		{
		}

		/// <summary>
		/// Adds the ticket locally.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public void addTicketLocally(Ticket ticket)
		{
			ticketDB.open();
			ticketDB.AddTicket(ticket);
			ticketDB.close();
		}

		public void destroyTicket(Ticket ticket)
		{
			Debug.WriteLine("TicketCell: Deleting ticket locally");
			deleteTicketLocally(ticket);
			Debug.WriteLine("TicketCell: Deleting ticket remotely");
			deleteTicketRemotely(ticket);

			Debug.WriteLine("TicketCell: Removing ticket from views");
			switch (ticket.mediaType)
			{
				case ("Picture"):
				case ("Photo"):
					TicketsPicture.pictureTickets.Remove(ticket);
					break;
				case ("Sound"):
				case ("Song"):
				case ("Audio"):
					TicketsSounds.soundTickets.Remove(ticket);
					break;
				case ("Video"):
				case ("YouTube"):
					TicketsVideos.videoTickets.Remove(ticket);
					break;
			}

			TicketsByPeriod.removeTicket(ticket);

			bool inPeriodList = false;
			foreach (Ticket t in DisplayTickets.displayTickets)
			{
				if (t.id == ticket.id)
				{
					inPeriodList = true;

				}
			}
			if (inPeriodList) { DisplayTickets.displayTickets.Remove(ticket); }

			Debug.WriteLine("TicketCell: Deleting ticket file locally");

			if (!(ticket.pathToFile.StartsWith("storage", StringComparison.Ordinal)))
			{
				var mediaController = new MediaController();
				mediaController.deleteFile(ticket.pathToFile);
			}
		}

		/// <summary>
		/// Deletes the ticket remotely.
		/// </summary>
		/// <returns><c>true</c>, if ticket was deleted remotely, <c>false</c> otherwise.</returns>
		/// <param name="ticket">Ticket.</param>
		public bool deleteTicketRemotely(Ticket ticket)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			var jobject = networkController.sendDeleteRequest("tickets/destroy", parameters);
			if (jobject == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the tickets attached to the active person.
		/// </summary>
		/// <returns>The tickets.</returns>
		public List<Ticket> getTickets()
		{
			ticketDB.open();
			var raw_tickets = ticketDB.getTicketsByPerson(Session.activePerson.id);
			ticketDB.close();

			return filterTicketsForUserType(raw_tickets);
		}

		/// <summary>
		/// Filters the type of the tickets for user.
		/// </summary>
		/// <returns>The tickets for user type.</returns>
		/// <param name="input">Input.</param>
		public List<Ticket> filterTicketsForUserType(List<Ticket> input)
		{
			var output = new List<Ticket>();

			var personController = new PersonController();
			var relation = personController.getRelation(Session.activePerson.id);
			if (relation.user_type.ToLower().Equals("admin"))
			{
				relation.user_type = "All";
			}

			IDictionary<string, int> groups = new Dictionary<string, int>();
			for (int i = 0; i < ProjectResource.groups.Length; i++)
			{
				groups.Add(ProjectResource.groups[i], i);
			}

			if (input.Count != 0)
			{
				var g_num = groups[relation.user_type];
				foreach (Ticket t in input)
				{
					Debug.WriteLine("TicketController: Ticket access level = " + t.access_level);
					if (groups[t.access_level] >= g_num)
					{
						output.Add(t);
					}
				}
			}

			return output;
		}

		/// <summary>
		/// Gets the tickets by person.
		/// </summary>
		/// <returns>The tickets by person.</returns>
		/// <param name="person_id">Person identifier.</param>
		public List<Ticket> getTicketsByPerson(int person_id)
		{
			ticketDB.open();
			var tickets = ticketDB.getTicketsByPerson(person_id);
			ticketDB.close();
			return tickets;
		}

		/// <summary>
		/// Gets the ticket.
		/// </summary>
		/// <returns>The ticket.</returns>
		/// <param name="id">Identifier.</param>
		public Ticket getTicket(int id)
		{
			ticketDB.open();
			var ticket = ticketDB.GetTicket(id);
			ticketDB.close();
			return ticket;
		}

		/// <summary>
		/// Deletes the ticket.
		/// </summary>
		/// <returns>The ticket.</returns>
		/// <param name="ticket">Ticket.</param>
		public void deleteTicketLocally(Ticket ticket)
		{
			ticketDB.open();
			ticketDB.DeleteTicket(ticket.id);
			ticketDB.close();
		}

		/// <summary>
		/// Updates the ticket locally.
		/// </summary>
		/// <returns>The ticket locally.</returns>
		/// <param name="ticket">Ticket.</param>
		public void updateTicketLocally(Ticket ticket)
		{
			ticketDB.open();
			ticketDB.DeleteTicket(ticket.id);
			ticketDB.AddTicket(ticket);
			ticketDB.close();
		}

		/// <summary>
		/// Updates the ticket remotely.
		/// </summary>
		/// <returns>The ticket remotely.</returns>
		/// <param name="ticket">Ticket.</param>
		public async Task<Ticket> updateTicketRemotely(Ticket ticket, string period)
		{
			IDictionary<string, string> paramters = new Dictionary<string, string>();
			paramters["ticket_id"] = ticket.id.ToString();
			paramters["title"] = ticket.title;
			paramters["description"] = ticket.description;
			paramters["year"] = ticket.year;
			paramters["area"] = ticket.area;
			paramters["access_level"] = ticket.access_level;
			paramters["period"] = period;
			paramters["token"] = Session.Token.val;

			//if (ticket.mediaType.Equals("Picture"))
			//{
			//	paramters["area"] = area;
			//}
			//else
			//{
			//	paramters["area"] = " ";
			//}

			var jobject = await networkController.sendPostRequest("tickets/update", paramters);
			if (jobject != null)
			{
				Debug.WriteLine("TicketController: Edited ticket returned - " + jobject);
				var jtoken = jobject.GetValue("Ticket");
				var returned = jtoken.ToObject<Ticket>();

				// Update ticket area relationships.
				//var ticketAreaDB = new TicketAreaDB();
				//var relations = ticketAreaDB.getRelationByTicketID(returned.id);
				//foreach (TicketArea ta in relations)
				//{
				//	ticketAreaDB.DeleteTicketArea(ta.id);
				//}

				//jtoken = jobject.GetValue("Area");
				//var rArea = jtoken.ToObject<Area>();
				//var areaController = new AreaController();
				//if (areaController.getArea(rArea.id) == null)
				//{
				//	areaController.addAreaLocally(rArea);
				//}
				//ticketAreaDB.AddTicketArea(new TicketArea(returned.id, returned.area_id));
				return returned;

				// TODO update ticket period relationship
			}

			return null;
		}

		/// <summary>
		/// Adds the tag relations locally.
		/// </summary>
		/// <returns>The tag relations locally.</returns>
		/// <param name="tags">Tags.</param>
		/// <param name="ticket">Ticket.</param>
		public void addTagRelationsLocally(List<Tag> tags, Ticket ticket)
		{
			TicketTagDB ttDB = new TicketTagDB();
			ttDB.open();
			foreach (Tag t in tags)
			{
				var ttr = new TicketTag(ticket.id, t.id);
				ttDB.AddTicketTagRelationship(ttr);
			}
			ttDB.close();
		}

		/// <summary>
		/// Check for new tickets from the server.
		/// </summary>
		/// <returns>The tickets.</returns>
		public async Task updateTicketsFromAPI()
		{
			// Set parameters
			Debug.WriteLine("Setting parameters");
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = Session.activePerson.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send GET request
			Console.WriteLine("Sending GET Request for all tickets");
			NetworkController net = new NetworkController();
			var jobject = await net.sendGetRequest("people/tickets", parameters);
			Console.WriteLine(jobject);

			// Parse JSON Areas to Areas
			//Console.WriteLine("Parsing areas");
			//var jtoken = jobject.GetValue("areas");
			//var areas = jtoken.ToObject<List<Area>>();
			////var areaDB = new AreaDB();
			//var areaController = new AreaController();
			//foreach (Area a in areas)
			//{
			//	Console.WriteLine(a);

			//	var storedArea = areaController.getArea(a.id);
			//	if (storedArea == null)
			//	{
			//		Console.WriteLine("New area... saving");
			//		//areaDB.AddArea(a);
			//		areaController.addAreaLocally(a);
			//	}
			//	else if (storedArea.GetHashCode() != a.GetHashCode())
			//	{
			//		Console.WriteLine("Updating area");
			//		areaController.updateAreaLocally(a);
			//	}
			//}
			//areaDB.close();

			// Parse JSON Tags to Tags
			Console.WriteLine("Parsing tags");
			var jtoken = jobject.GetValue("tags");
			Console.WriteLine(jtoken);
			var tags = jtoken.ToObject<Tag[]>();
			var tagController = new TagController();
			//var tagDB = new TagDB();
			foreach (Tag t in tags)
			{
				Console.WriteLine(t);
				var storedTag = tagController.getTag(t.id);
				//var storedTag = tagDB.GetTag(t.id);
				if (storedTag == null)
				{
					Console.WriteLine("New tag... saving");
					//tagDB.AddTag(t);
					tagController.addTagLocally(t);

				}
				else if (storedTag.GetHashCode() != t.GetHashCode())
				{
					Console.WriteLine("Updating tag");
					tagController.updateTagLocally(t);
					//tagDB.DeleteTag(t.id);
					//tagDB.AddTag(t);

				}
			}
			//tagDB.close();

			// Parse JSON Tickets to Tickets
			Console.WriteLine("Parsing tickets");
			jtoken = jobject.GetValue("tickets");
			Console.WriteLine(jtoken);
			var tickets = jtoken.ToObject<Ticket[]>();
			//var ticketDB = new TicketDB();
			//ticketDB.open();
			foreach (Ticket t in tickets)
			{
				Console.WriteLine(t);

				//var storedTicket = ticketDB.GetTicket(t.id);
				var storedTicket = getTicket(t.id);
				if (storedTicket == null)
				{
					Console.WriteLine("New ticket... saving");
					//ticketDB.AddTicket(t);
					addTicketLocally(t);

					//foreach (Tag tag in t.tags)
					//{
					//	TicketTag tt = new TicketTag(t.id, tag.id);
					//	ticketTagDB.AddTicketTagRelationship(tt);
					//}
				}
				else if (storedTicket.GetHashCode() != t.GetHashCode())
				{
					Console.WriteLine("Updating ticket");
					updateTicketLocally(t);
				}
			}
			//ticketDB.close();

			// Parse JSON TicketTags to TicketTags
			Console.WriteLine("Parsing ticket_tags");
			jtoken = jobject.GetValue("ticket_tags");
			Console.WriteLine(jtoken);
			var ticket_tags = jtoken.ToObject<TicketTag[]>();
			var ticketTagDB = new TicketTagDB();
			ticketTagDB.open();
			foreach (TicketTag tt in ticket_tags)
			{
				var stored = ticketTagDB.getRelationByTicketAndTagID(tt.ticket_id, tt.tag_id);
				if (stored == null)
				{
					Console.WriteLine("New ticket_tag, adding...");
					ticketTagDB.AddTicketTagRelationship(tt);
				}
				else if (stored.GetHashCode() != tt.GetHashCode())
				{
					Console.WriteLine("Updating ticket_tag");
					ticketTagDB.DeleteRelation(stored.id);
					ticketTagDB.AddTicketTagRelationship(tt);
				}
			}
			ticketTagDB.close();
		}

		/// <summary>
		/// Gets the ticket image.
		/// </summary>
		/// <returns>The ticket image.</returns>
		/// <param name="ticket">Ticket.</param>
		public Image getTicketImage(Ticket ticket)
		{
			bool download_finished = false;
			Image ticket_photo = new Image();
			if (ticket.pathToFile.StartsWith("storage", StringComparison.Ordinal))
			{
				NetworkController net = new NetworkController();

				var fileName = String.Empty;
				switch (ticket.mediaType)
				{
					case ("Picture"):
						fileName = "t_" + ticket.id + ".jpg";
						break;
					case ("Sound"):
						fileName = "t_" + ticket.id + ".wav";
						break;
				}

				var task = Task.Run(() => net.downloadFile(ticket.pathToFile, fileName)).Result;
				ticket.pathToFile = fileName;

				while (!download_finished)
				{
				}

				ticket_photo.Source = ImageSource.FromStream(() => new MemoryStream(MediaController.readBytesFromFile(ticket.pathToFile)));

				var ticketController = new TicketController();
				ticketController.updateTicketLocally(ticket);
			}
			else
			{
				ticket_photo.Source = ImageSource.FromStream(() => new MemoryStream(MediaController.readBytesFromFile(ticket.pathToFile)));
			}

			return ticket_photo;
		}

		/// <summary>
		/// extracts video code from the url
		/// </summary>
		/// <returns>The youtube to ticket.</returns>
		/// <param name="link">Link.</param>
		public Ticket parseYouTubeToTicket(string link)
		{

			if (link.Contains("youtu.be"))
			{
				var idx = link.LastIndexOf("/", StringComparison.Ordinal);
				var videoCode = link.Substring(idx + 1);
				return new Ticket
				{
					pathToFile = videoCode,
					mediaType = "YouTube"
				};
			}
			else
			{
				var idx = link.LastIndexOf("=", StringComparison.Ordinal);
				var videoCode = link.Substring(idx + 1);
				return new Ticket
				{
					pathToFile = videoCode,
					mediaType = "YouTube"
				};
			}
		}

		/// <summary>
		/// Downloads the content of the ticket.
		/// </summary>
		/// <returns>The ticket content.</returns>
		/// <param name="filePath">File path.</param>
		public async Task downloadTicketContent(string filePath)
		{

			//var download_finished = false;

			//MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			//{
			//	Debug.WriteLine("Image Downloaded");
			//	download_finished = finished;
			//});

			NetworkController net = new NetworkController();

			var idx = filePath.LastIndexOf("/", StringComparison.Ordinal);
			var fileName = filePath.Substring(idx + 1);

			await net.downloadFile(filePath, fileName);

			//while (!download_finished)
			//{
			//}

			//MessagingCenter.Unsubscribe<NetworkController, bool>(this, "download_image");
		}

		/// <summary>
		/// Gets the display string.
		/// </summary>
		/// <returns>The display string.</returns>
		/// <param name="ticket">Ticket.</param>
		public string getDisplayString(Ticket ticket)
		{
			var displayString = string.Empty;
			//var areaController = new AreaController();
			//var area = areaController.getArea(ticket.area_id);

			switch (ticket.mediaType)
			{
				case ("Picture"):
					displayString = string.Format("Taken in {0}, {1}", ticket.area, ticket.year);
					break;
				default:
					displayString = string.Format("From {0}", ticket.year);
					break;
			}
			return displayString;
		}

		/// <summary>
		/// Updates the display ticket.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public void updateDisplayTicket(Ticket ticket)
		{
			ViewTicket.displayedTicket.title = ticket.title;
			ViewTicket.displayedTicket.description = ticket.description;
			ViewTicket.displayedTicket.displayString = getDisplayString(ticket);

			ViewTicket.displayedTicket.area = ticket.area;
		}
	}
}

