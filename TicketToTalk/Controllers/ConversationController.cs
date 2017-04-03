using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TicketToTalk
{
	/// <summary>
	/// Controller for the conversation model.
	/// </summary>
	public class ConversationController
	{
		NetworkController networkController = new NetworkController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationController"/> class.
		/// </summary>
		public ConversationController()
		{
		}

		/// <summary>
		/// Stores the conversation locally.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public void StoreConversationLocally(Conversation conversation)
		{
			lock (Session.Connection)
			{
				Session.Connection.Insert(conversation);
			}
		}

		/// <summary>
		/// Gets the conversation.
		/// </summary>
		/// <returns>The conversation.</returns>
		/// <param name="id">Identifier.</param>
		public Conversation GetConversationLocally(int id)
		{
			Conversation conversation;

			lock (Session.Connection)
			{
				conversation = (from n in Session.Connection.Table<Conversation>() where n.id == id select n).FirstOrDefault();
			}

			return conversation;
		}

		/// <summary>
		/// Gets the conversations.
		/// </summary>
		/// <returns>The conversations.</returns>
		public List<Conversation> GetLocalConversations()
		{
			List<Conversation> convs = new List<Conversation>();

			lock (Session.Connection)
			{
				var q = from c in Session.Connection.Table<Conversation>() where c.person_id == Session.activePerson.id select c;

				foreach (Conversation c in q)
				{
					convs.Add(c);
				}
			}

			return convs;
		}

		/// <summary>
		/// Stores the conversation remotely.
		/// </summary>
		/// <returns>The new conversation.</returns>
		/// <param name="conversation">Conversation.</param>
		public async Task<Conversation> StoreConversationRemotely(Conversation conversation)
		{
			// Get platform of the device.
			var platform = string.Empty;
#if __IOS__
			platform = "iOS";
#else
			platform = "Android";
#endif

			// Build the paramters.
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["datetime"] = conversation.date;
			parameters["ticket_id_string"] = conversation.ticket_id_string;
			parameters["platform"] = platform;
			parameters["notes"] = conversation.notes;
			parameters["person_id"] = conversation.person_id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			JObject jobject = null;

			try
			{
				jobject = await networkController.SendPostRequest("conversations/store", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// If null, the request failed.
			if (jobject == null)
			{
				return null;
			}
			else
			{
				var data = jobject.GetData();
				var jtoken = data["conversation"];
				var conv = jtoken.ToObject<Conversation>();
				return conv;
			}
		}

		/// <summary>
		/// Updates the conversation remotely.
		/// </summary>
		/// <returns>The updated conversation.</returns>
		/// <param name="conversation">Conversation.</param>
		public async Task<bool> UpdateConversationRemotely(Conversation conversation)
		{
			// Build the parameters.
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["notes"] = conversation.notes;
			parameters["datetime"] = conversation.date;
			parameters["token"] = Session.Token.val;

			// Send the request.
			JObject jobject = null;

			try
			{
				jobject = await networkController.SendPostRequest("conversations/update", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// If null, request failed.
			if (jobject != null)
			{
				var data = jobject.GetData();

				var returned = data["conversation"].ToObject<Conversation>();
				returned = SetPropertiesForDisplay(returned);
				UpdateConversationViews(returned);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Updates the conversation locally.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public void UpdateConversationViews(Conversation conversation)
		{

			var idx = -1;
			for (int i = 0; i < ConversationsView.conversations.Count; i++)
			{
				if (conversation.id == ConversationsView.conversations[i].id)
				{
					idx = i;
					break;
				}
			}

			ConversationsView.conversations[idx] = conversation;

			idx = -1;
			for (int i = 0; i < ConversationSelect.conversations.Count; i++)
			{
				if (conversation.id == ConversationSelect.conversations[i].id)
				{
					idx = i;
					break;
				}
			}
			if (idx > 1) 
			{
				ConversationSelect.conversations[idx] = conversation;
			}

			//ConversationView.conversation = conversation;
			ConversationView.conversation.date = conversation.date;
			ConversationView.conversation.timestamp = conversation.timestamp;
			ConversationView.conversation.displayDate = conversation.displayDate;
			ConversationView.conversation.notes = conversation.notes;

			// Store in local DB.

			UpdateLocalConversation(conversation);
		}

		public void UpdateLocalConversation(Conversation conversation)
		{
			lock (Session.Connection)
			{
				Session.Connection.Update(conversation);
			}
		}

		/// <summary>
		/// Deletes the conversation locally.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public void DeleteConversationLocally(Conversation conversation)
		{
			lock (Session.Connection)
			{
				Session.Connection.Delete(conversation);
			}
		}

		/// <summary>
		/// Deletes the conversation remotely.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public async Task<bool> DeleteConversationRemotely(Conversation conversation)
		{
			// Build paramters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send request.
			JObject jobject = null;

			try
			{
				jobject = await networkController.SendGetRequest("conversations/destroy", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// If null, the request failed.
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
		/// Destroies the conversation.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public async Task DestroyConversation(Conversation conversation)
		{
			// Delete conversation remotely.
			bool deleted = false;

			try
			{
				deleted = await DeleteConversationRemotely(conversation);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// If successfully deleted...
			if (deleted)
			{
				DeleteConversationLocally(conversation);
				ConversationsView.conversations.Remove(conversation);
			}
		}

		/// <summary>
		/// Get a list of conversations for this person-user relationship from the server.
		/// </summary>
		/// <returns>The remote conversations.</returns>
		public async static Task<List<Conversation>> GetRemoteConversations(ConversationController instance)
		{
			// Build paramters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = Session.activePerson.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			JObject jobject = null;
			var networkController = new NetworkController();

			try
			{
				jobject = await networkController.SendGetRequest("conversations/get", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// If null, request failed.
			if (jobject == null)
			{
				return null;
			}
			else
			{
				var data = jobject.GetData();
				var jtoken = data["conversations"];
				var conv = jtoken.ToObject<List<Conversation>>();
				return conv;
			};
		}

		/// <summary>
		/// Adds the ticket to conversation locally.
		/// </summary>
		/// <returns>The conversation with the added ticket.</returns>
		/// <param name="conversation">Conversation.</param>
		/// <param name="ticket">Ticket.</param>
		public Conversation AddTicketToConversation(Conversation conversation, Ticket ticket)
		{

			conversation.ticket_id_string = conversation.ticket_id_string.Trim();

			// If the ticket string is empty, the ticket string becomes the ticket id.
			if ((string.IsNullOrEmpty(conversation.ticket_id_string)))
			{
				conversation.ticket_id_string = ticket.id.ToString();
				return conversation;
			}

			// Split the string for an array of all ticket ids.
			//char[] delims = { ' ' };
			//string[] str = conversation.ticket_id_string.Split(delims);

			// Convert list into string.
			var list = new List<string>(conversation.ticket_id_string.Split(' '));

			// Add new id to the list.
			list.Add(ticket.id.ToString());

			var str = "";
			foreach (string s in list)
			{
				str += string.Format("{0} ", s);
			}

			conversation.ticket_id_string = str.Trim();
			return conversation;
		}

		/// <summary>
		/// Removes the ticket from conversation.
		/// </summary>
		/// <returns>The ticket from conversation.</returns>
		/// <param name="conversation">Conversation.</param>
		/// <param name="ticket">Ticket.</param>
		public Conversation RemoveTicketFromConversation(Conversation conversation, Ticket ticket)
		{
			conversation.ticket_id_string = conversation.ticket_id_string.Trim();
			var ticket_ids = new List<string>(conversation.ticket_id_string.Split(' '));

			ticket_ids.Remove(ticket.id.ToString());

			var str = string.Join(" ", ticket_ids.ToArray());

			Debug.WriteLine(str);

			conversation.ticket_id_string = str;

			return conversation;
		}

		/// <summary>
		/// Adds the ticket to conversation remotely.
		/// </summary>
		/// <returns>The ticket to conversation remotely.</returns>
		/// <param name="conversation">Conversation.</param>
		/// <param name="ticket">Ticket.</param>
		public async Task<bool> AddTicketToConversationRemotely(Conversation conversation, Ticket ticket)
		{
			// Build the parameters.
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			JObject jobject = null;

			try
			{
				jobject = await networkController.SendPostRequest("conversations/tickets/add", parameters);

			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// If null, the request failed.
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
		/// Removes the ticket from conversation remotely.
		/// </summary>
		/// <returns>The conversation</returns>
		/// <param name="conversation">Conversation.</param>
		/// <param name="ticket">Ticket.</param>
		public async Task<bool> RemoveTicketFromConversationRemotely(Conversation conversation, Ticket ticket)
		{

			// Build parameters.
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send request.

			JObject jobject = null;

			try
			{
				jobject = await networkController.SendPostRequest("conversations/tickets/remove", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// If null, request failed.
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
		/// Sets the properties for display.
		/// 
		/// Creates a formatted string for date and time. Also calculates number of tickets in the conversation.
		/// </summary>
		/// <returns>The properties for display.</returns>
		/// <param name="conversation">Conversation.</param>
		public Conversation SetPropertiesForDisplay(Conversation conversation)
		{

			char[] delims = { ' ' };

			conversation.displayDate = formatTimestamp(conversation.timestamp);

			if (!string.IsNullOrEmpty(conversation.ticket_id_string))
			{
				List<string> ticket_ids = new List<string>(conversation.ticket_id_string.Split(delims));
				var t_count = 0;
				foreach (string t in ticket_ids)
				{
					if (!(string.IsNullOrEmpty(t)))
					{
						t_count++;
					}
				}

				conversation.ticketCount = t_count;
			}
			else
			{
				conversation.ticketCount = 0;
			}

			return conversation;
		}

		/// <summary>
		/// Formats the timestamp.
		/// </summary>
		/// <returns>The timestamp.</returns>
		/// <param name="timestamp">Timestamp.</param>
		public string formatTimestamp(DateTime timestamp) 
		{

			string[] months =
			{
				"January",
				"February",
				"March",
				"April",
				"May",
				"June",
				"July",
				"August",
				"September",
				"October",
				"November",
				"December"
			};

			var str = string.Format("{0} {1}, {2}", months[timestamp.Month - 1], timestamp.Day, timestamp.Year);

			return str;
		}

		/// <summary>
		/// Adds the ticket to displayed conversation.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public void AddTicketToDisplayedConversation(Conversation conversation, Ticket ticket)
		{
			var item = new ConversationItem(conversation, ticket);

			ConversationView.conversationItems.Add(item);
			ConversationView.tickets.Add(ticket);

			foreach (Conversation c in ConversationsView.conversations)
			{
				if (c.id == conversation.id)
				{
					conversation.ticketCount++;
					break;
				}
			}
		}

		/// <summary>
		/// Parses the date to integers.
		/// </summary>
		/// <returns>The date to integers.</returns>
		/// <param name="conversation">Conversation.</param>
		public int[] ParseDateToIntegers(Conversation conversation)
		{
			char[] delims = { '/', ':', ' ' };
			string[] sDates = conversation.date.Split(delims);
			int[] dates = new int[sDates.Length];
			for (int i = 0; i < sDates.Length; i++)
			{
				dates[i] = int.Parse(sDates[i]);
			}

			return dates;
		}
	}
}

