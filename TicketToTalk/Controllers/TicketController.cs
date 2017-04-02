using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Ticket controller.
	/// </summary>
	public class TicketController
	{
		//private TicketDB ticketDB = new TicketDB();
		private NetworkController networkController = new NetworkController();

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
		public void AddTicketLocally(Ticket ticket)
		{

			lock (Session.connection)
			{
				Session.connection.Insert(ticket);
			}
		}

		/// <summary>
		/// Adds the ticket remotely.
		/// </summary>
		/// <returns>The new ticket.</returns>
		/// <param name="ticket">Ticket.</param>
		/// <param name="media">Media.</param>
		/// <param name="period">Period.</param>
		public async Task<Ticket> AddTicketRemotely(Ticket ticket, byte[] media, Period period)
		{

			// Create parameters for the request.
			var net = new NetworkController();
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["token"] = Session.Token.val;
			parameters["ticket"] = ticket;
			parameters["media"] = media;
			parameters["period"] = period;

			// Send the request
			var jobject = await net.SendPostRequest("tickets/store", parameters);

			if (jobject != null)
			{

				var data = jobject.GetData();

				// Gets the ticket object.
				var returned_ticket = data["ticket"].ToObject<Ticket>();

				// Add to the ticket displays.
				var ticketController = new TicketController();
				returned_ticket.displayString = ticketController.GetDisplayString(returned_ticket);
				string ext = string.Empty;
				switch (ticket.mediaType)
				{
					case ("Picture"):
						returned_ticket.displayIcon = "photo_icon.png";
						ext = ".jpg";
						returned_ticket.pathToFile = "t_" + returned_ticket.id + ext;
						TicketsPicture.pictureTickets.Add(returned_ticket);
						break;
					case ("Sound"):
						returned_ticket.displayIcon = "audio_icon.png";
						ext = ".wav";
						returned_ticket.pathToFile = "t_" + returned_ticket.id + ext;
						TicketsSounds.soundTickets.Add(returned_ticket);
						break;
					case ("Video"):
					case ("YouTube"):
						returned_ticket.displayIcon = "video_icon.png";
						TicketsVideos.videoTickets.Add(returned_ticket);
						break;
				}

				// Save the file.
				MediaController.WriteImageToFile("t_" + returned_ticket.id + ext, media);

				ticketController.AddTicketLocally(returned_ticket);

				// Add to view
				TicketsByPeriod.AddTicket(returned_ticket);

				return returned_ticket;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Destroies the ticket.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public async Task<bool> DestroyTicket(Ticket ticket)
		{

			var deleted = false;

			try
			{
				deleted = await DeleteTicketRemotely(ticket);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			if (deleted) 
			{
				// Delete the tickets.
				DeleteTicketLocally(ticket);

				// Remove the tickets from views.
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

				TicketsByPeriod.RemoveTicket(ticket);

				bool inPeriodList = false;
				foreach (Ticket t in DisplayTickets.displayTickets)
				{
					if (t.id == ticket.id)
					{
						inPeriodList = true;

					}
				}
				if (inPeriodList) { DisplayTickets.displayTickets.Remove(ticket); }

				if (!(ticket.pathToFile.StartsWith("ticket_to_talk", StringComparison.Ordinal)))
				{
					var mediaController = new MediaController();
					mediaController.DeleteFile(ticket.pathToFile);
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the tickets by period identifier.
		/// </summary>
		/// <returns>The tickets by period identifier.</returns>
		/// <param name="period_id">Period identifier.</param>
		public List<Ticket> GetTicketsByPeriodID(int period_id)
		{
			List<Ticket> tickets = new List<Ticket>();

			lock (Session.connection)
			{
				var q = from t in Session.connection.Table<Ticket>() where t.period_id == period_id select t;

				foreach (Ticket t in q) 
				{
					tickets.Add(t);
				}
			}

			return tickets;
		}

		/// <summary>
		/// Deletes the ticket remotely.
		/// </summary>
		/// <returns><c>true</c>, if ticket was deleted remotely, <c>false</c> otherwise.</returns>
		/// <param name="ticket">Ticket.</param>
		public async Task<bool> DeleteTicketRemotely(Ticket ticket)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			JObject jobject = null;

			try
			{
				jobject = await networkController.SendDeleteRequest("tickets/destroy", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

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
		public List<Ticket> GetTickets()
		{
			List<Ticket> tickets = new List<Ticket>();

			lock (Session.connection)
			{
				var q = from t in Session.connection.Table<Ticket>() where t.person_id == Session.activePerson.id select t;

				foreach (Ticket t in q) 
				{
					tickets.Add(t);
				}
			}

			return FilterTicketsForUserType(tickets);
		}

		/// <summary>
		/// Filters the type of the tickets for user.
		/// </summary>
		/// <returns>The tickets for user type.</returns>
		/// <param name="input">Input.</param>
		public List<Ticket> FilterTicketsForUserType(List<Ticket> input)
		{
			var output = new List<Ticket>();

			var personController = new PersonController();
			var relation = personController.GetRelation(Session.activePerson.id);
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
		public List<Ticket> GetTicketsByPerson(int person_id)
		{
			List<Ticket> tickets = new List<Ticket>();

			lock (Session.connection)
			{
				var q = from t in Session.connection.Table<Ticket>() where t.person_id == Session.activePerson.id select t;

				foreach (Ticket t in q)
				{
					tickets.Add(t);
				}
			}

			return tickets;
		}

		/// <summary>
		/// Gets the ticket.
		/// </summary>
		/// <returns>The ticket.</returns>
		/// <param name="id">Identifier.</param>
		public Ticket GetTicket(int id)
		{
			Ticket ticket;

			lock (Session.connection)
			{
				ticket = (from t in Session.connection.Table<Ticket>() where t.id == id select t).FirstOrDefault();
			}

			return ticket;
		}

		/// <summary>
		/// Deletes the ticket.
		/// </summary>
		/// <returns>The ticket.</returns>
		/// <param name="ticket">Ticket.</param>
		public void DeleteTicketLocally(Ticket ticket)
		{
			lock (Session.connection)
			{
				Session.connection.Delete(ticket);
			}
		}

		/// <summary>
		/// Updates the ticket locally.
		/// </summary>
		/// <returns>The ticket locally.</returns>
		/// <param name="ticket">Ticket.</param>
		public void UpdateTicketLocally(Ticket ticket)
		{
			lock (Session.connection)
			{
				Session.connection.Update(ticket);
			}
		}

		/// <summary>
		/// Updates the ticket remotely.
		/// </summary>
		/// <returns>The ticket remotely.</returns>
		/// <param name="ticket">Ticket.</param>
		public async Task<Ticket> UpdateTicketRemotely(Ticket ticket, string period)
		{
			IDictionary<string, object> paramters = new Dictionary<string, object>();
			paramters["ticket_id"] = ticket.id.ToString();
			paramters["title"] = ticket.title;
			paramters["description"] = ticket.description;
			paramters["year"] = ticket.year;
			paramters["area"] = ticket.area;
			paramters["access_level"] = ticket.access_level;
			paramters["period"] = period;
			paramters["token"] = Session.Token.val;

			JObject jobject = null;

			try
			{
				jobject = await networkController.SendPostRequest("tickets/update", paramters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			if (jobject != null)
			{

				var data = jobject.GetData();
				var returned = data["ticket"].ToObject<Ticket>();

				return returned;
			}

			return null;
		}

		/// <summary>
		/// Adds the tag relations locally.
		/// </summary>
		/// <returns>The tag relations locally.</returns>
		/// <param name="tags">Tags.</param>
		/// <param name="ticket">Ticket.</param>
		public void AddTagRelationsLocally(List<Tag> tags, Ticket ticket)
		{
			lock (Session.connection)
			{
				foreach (Tag t in tags)
				{
					var ttr = new TicketTag(ticket.id, t.id);
					Session.connection.Insert(ttr);
				}
			}
		}

		/// <summary>
		/// Check for new tickets from the server.
		/// </summary>
		/// <returns>The tickets.</returns>
		public async Task GetRemoteTickets()
		{
			// Set parameters
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = Session.activePerson.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send GET request
			var net = new NetworkController();

			JObject jobject = null;

			try
			{
				jobject = await net.SendGetRequest("people/tickets", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}


			var data = jobject.GetData();

			// Parse JSON Tags to Tags
			var tags = data["tags"].ToObject<Tag[]>();
			var tagController = new TagController();

			foreach (Tag t in tags)
			{
				Console.WriteLine(t);
				var storedTag = tagController.GetTag(t.id);
				if (storedTag == null)
				{
					Console.WriteLine("New tag... saving");
					tagController.AddTagLocally(t);

				}
				else if (storedTag.GetHashCode() != t.GetHashCode())
				{
					tagController.UpdateTagLocally(t);
				}
			}

			// Parse JSON Tickets to Tickets
			var tickets = data["tickets"].ToObject<Ticket[]>();

			foreach (Ticket t in tickets)
			{
				var storedTicket = GetTicket(t.id);
				if (storedTicket == null)
				{
					AddTicketLocally(t);
				}
				else if (storedTicket.GetHashCode() != t.GetHashCode())
				{
					UpdateTicketLocally(t);
				}
			}

			// Parse JSON TicketTags to TicketTags
			var ticket_tags = data["ticket_tags"].ToObject<TicketTag[]>();

			lock (Session.connection)
			{
				foreach (TicketTag tt in ticket_tags)
				{
					var stored = (from t in Session.connection.Table<TicketTag>() where t.ticket_id == tt.ticket_id && t.tag_id == tt.tag_id select t).FirstOrDefault();

					//var stored = ticketTagDB.GetRelationByTicketAndTagID(tt.ticket_id, tt.tag_id);
					if (stored == null)
					{
						Console.WriteLine("New ticket_tag, adding...");
						Session.connection.Insert(tt);
					}
					else if (stored.GetHashCode() != tt.GetHashCode())
					{
						Console.WriteLine("Updating ticket_tag");
						Session.connection.Update(tt);
					}
				}
			}
		}

		/// <summary>
		/// Gets the ticket image.
		/// </summary>
		/// <returns>The ticket image.</returns>
		/// <param name="ticket">Ticket.</param>
		public Image GetTicketImage(Ticket ticket)
		{
			bool download_finished = false;
			var ticket_photo = new Image();
			if (ticket.pathToFile.StartsWith("ticket_to_talk", StringComparison.Ordinal))
			{
				var net = new NetworkController();

				var fileName = string.Empty;
				switch (ticket.mediaType)
				{
					case ("Picture"):
						fileName = "t_" + ticket.id + ".jpg";
						break;
					case ("Sound"):
						fileName = "t_" + ticket.id + ".wav";
						break;
				}

				var task = Task.Run(() => net.DownloadFile(ticket.pathToFile, fileName)).Result;
				ticket.pathToFile = fileName;

				while (!download_finished)
				{
				}

				ticket_photo.Source = ImageSource.FromStream(() => new MemoryStream(MediaController.ReadBytesFromFile(ticket.pathToFile)));

				var ticketController = new TicketController();
				ticketController.UpdateTicketLocally(ticket);
			}
			else
			{
				ticket_photo.Source = ImageSource.FromStream(() => new MemoryStream(MediaController.ReadBytesFromFile(ticket.pathToFile)));
			}

			return ticket_photo;
		}

		/// <summary>
		/// extracts video code from the url
		/// </summary>
		/// <returns>The youtube to ticket.</returns>
		/// <param name="link">Link.</param>
		public Ticket ParseYouTubeToTicket(string link)
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
		/// <exception cref="T:System.IO.FileNotFoundException"></exception>
		public async Task DownloadTicketContent(Ticket ticket)
		{
			var fileName = String.Format("t_{0}.jpg", ticket.id);
			var client = new HttpClient();

			client.DefaultRequestHeaders.Host = "tickettotalk.openlab.ncl.ac.uk";
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			client.Timeout = new TimeSpan(0, 0, 100);

			var url = new Uri(Session.baseUrl + "tickets/download?ticket_id=" + ticket.id + "&token=" + Session.Token.val + "&api_key=" + Session.activeUser.api_key);

			Console.WriteLine("Beginning Download");
			Stream returned = null;

			try
			{
				returned = await client.GetStreamAsync(url);
			}
			catch (WebException ex)
			{
				Debug.WriteLine(ex.StackTrace);
				throw new NoNetworkException("No network available, check you are connected to the internet.");
			}
			catch (TaskCanceledException ex)
			{
				Debug.WriteLine(ex.StackTrace);
				throw new NoNetworkException("No network available, check you are connected to the internet.");
			}
			catch (HttpRequestException ex)
			{
				Debug.WriteLine(ex.StackTrace);
				throw new NoNetworkException("No network available, check you are connected to the internet.");
			}

			byte[] buffer = new byte[16 * 1024];
			byte[] imageBytes;
			using (MemoryStream ms = new MemoryStream())
			{
				int read = 0;
				while ((read = returned.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				imageBytes = ms.ToArray();
			}

			if (returned != null)
			{
				MediaController.WriteImageToFile(fileName, imageBytes);
			}
			else
			{
				//throw FileNotFoundException;
			}

		}

		/// <summary>
		/// Gets the display string.
		/// </summary>
		/// <returns>The display string.</returns>
		/// <param name="ticket">Ticket.</param>
		public string GetDisplayString(Ticket ticket)
		{
			var displayString = string.Empty;

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
		public void UpdateDisplayTicket(Ticket ticket)
		{
			ViewTicket.displayedTicket.title = ticket.title;
			ViewTicket.displayedTicket.description = ticket.description;
			ViewTicket.displayedTicket.displayString = GetDisplayString(ticket);

			ViewTicket.displayedTicket.area = ticket.area;
		}
	}
}

