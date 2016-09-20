using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TicketToTalk
{
	/// <summary>
	/// Controller for the conversation model.
	/// </summary>
	public class ConversationController
	{
		NetworkController networkController = new NetworkController();
		ConversationDB convDB = new ConversationDB();

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
		public void storeConversationLocally(Conversation conversation)
		{
			convDB.open();
			convDB.AddConversation(conversation);
			convDB.close();
		}

		/// <summary>
		/// Gets the conversation.
		/// </summary>
		/// <returns>The conversation.</returns>
		/// <param name="id">Identifier.</param>
		public Conversation getConversationLocally(int id)
		{
			convDB.open();
			var conversation = convDB.GetConversation(id);
			convDB.close();

			return conversation;
		}

		/// <summary>
		/// Gets the conversations.
		/// </summary>
		/// <returns>The conversations.</returns>
		public List<Conversation> getConversationsRemotely()
		{
			convDB.open();
			var convs = new List<Conversation>(convDB.GetConversationsForPerson());
			convDB.close();

			return convs;
		}

		/// <summary>
		/// Stores the conversation remotely.
		/// </summary>
		/// <returns>The new conversation.</returns>
		/// <param name="conversation">Conversation.</param>
		public async Task<Conversation> storeConversationRemotely(Conversation conversation)
		{
			// Get platform of the device.
			// TODO: Remove after fix for getting standard dates across language settings.
			var platform = string.Empty;
#if __IOS__
			platform = "iOS";
#else
			platform = "Android";
#endif

			// Build the paramters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["datetime"] = conversation.date;
			parameters["ticket_id_string"] = conversation.ticket_id_string;
			parameters["platform"] = platform;
			parameters["notes"] = conversation.notes;
			parameters["person_id"] = conversation.person_id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.sendPostRequest("conversations/store", parameters);

			// If null, the request failed.
			if (jobject == null)
			{
				return null;
			}
			else
			{
				var jtoken = jobject.GetValue("conversation");
				var conv = jtoken.ToObject<Conversation>();
				return conv;
			}
		}

		/// <summary>
		/// Updates the conversation remotely.
		/// </summary>
		/// <returns>The updated conversation.</returns>
		/// <param name="conversation">Conversation.</param>
		public async Task<Conversation> updateConversationRemotely(Conversation conversation)
		{
			// Build the parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["notes"] = conversation.notes;
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.sendPostRequest("conversations/update", parameters);

			// If null, request failed.
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("Conversation");
				return jtoken.ToObject<Conversation>();
			}

			return null;
		}

		/// <summary>
		/// Updates the conversation locally.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public void updateConversationLocally(Conversation conversation)
		{
			// Update conversation currently being displayed.
			foreach (Conversation c in ConversationsView.conversations)
			{
				if (c.id == conversation.id)
				{
					c.notes = conversation.notes;
				}
			}

			// Store in local DB.
			convDB.open();
			convDB.DeleteConversation(conversation.id);
			convDB.AddConversation(conversation);
			convDB.close();
		}

		/// <summary>
		/// Deletes the conversation locally.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public void deleteConversationLocally(Conversation conversation)
		{
			convDB.open();
			convDB.DeleteConversation(conversation.id);
			convDB.close();
		}

		/// <summary>
		/// Deletes the conversation remotely.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public async Task<bool> deleteConversationRemotely(Conversation conversation)
		{
			// Build paramters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send request.
			var jobject = await networkController.sendGetRequest("conversations/destroy", parameters);

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
		public async void destroyConversation(Conversation conversation)
		{
			// Delete conversation remotely.
			var deleted = await deleteConversationRemotely(conversation);

			// If successfully deleted...
			if (deleted)
			{
				deleteConversationLocally(conversation);
				ConversationsView.conversations.Remove(conversation);
			}
		}

		/// <summary>
		/// Get a list of conversations for this person-user relationship from the server.
		/// </summary>
		/// <returns>The remote conversations.</returns>
		public async Task<List<Conversation>> getRemoteConversations()
		{
			// Build paramters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = Session.activePerson.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.sendGetRequest("conversations/get", parameters);

			// If null, request failed.
			if (jobject == null)
			{
				return null;
			}
			else
			{
				var jtoken = jobject.GetValue("conversations");
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
		public Conversation addTicketToConversation(Conversation conversation, Ticket ticket)
		{

			// If the ticket string is empty, the ticket string becomes the ticket id.
			if ((string.IsNullOrEmpty(conversation.ticket_id_string)))
			{
				conversation.ticket_id_string = ticket.id.ToString();
				return conversation;
			}

			// Split the string for an array of all ticket ids.
			char[] delims = { ' ' };
			string[] str = conversation.ticket_id_string.Split(delims);

			// Convert list into string.
			var list = new List<string>(str);

			// Add new id to the list.
			list.Add(ticket.id.ToString());

			var temp = "";
			foreach (string s in list)
			{
				Debug.WriteLine(string.Format("ConversationController: conversation_id {0} ticket_id {1}", conversation.id, ticket.id));
				temp = string.Format("{0} ", s);
			}

			conversation.ticket_id_string = temp.TrimEnd();
			Debug.WriteLine("ConversationController: tickets = " + conversation.ticket_id_string);
			return conversation;
		}

		/// <summary>
		/// Removes the ticket from conversation.
		/// </summary>
		/// <returns>The ticket from conversation.</returns>
		/// <param name="conversation">Conversation.</param>
		/// <param name="ticket">Ticket.</param>
		public Conversation removeTicketFromConversation(Conversation conversation, Ticket ticket)
		{
			return null;
		}

		/// <summary>
		/// Adds the ticket to conversation remotely.
		/// </summary>
		/// <returns>The ticket to conversation remotely.</returns>
		/// <param name="conversation">Conversation.</param>
		/// <param name="ticket">Ticket.</param>
		public async Task<bool> addTicketToConversationRemotely(Conversation conversation, Ticket ticket)
		{
			// Build the parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.sendPostRequest("conversations/tickets/add", parameters);

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
		public async Task<bool> removeTicketFromConversationRemotely(Conversation conversation, Ticket ticket)
		{

			// Build parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send request.
			var jobject = await networkController.sendPostRequest("conversations/tickets/remove", parameters);

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
		public Conversation setPropertiesForDisplay(Conversation conversation)
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

			string displayString;
			string day, month, year, hour, minutes, date_suffix;
			int afterMid;

			char[] delims = { ' ' };
			string[] datetime = conversation.date.Split(delims);

			if (conversation.date.Contains("/"))
			{
				char[] d_delims = { '/' };
				string[] date = datetime[0].Split(d_delims);

#if __IOS__

				day = date[0];
				Debug.WriteLine("day: " + date[0]);
				Debug.WriteLine("month: " + date[1]);
				month = months[Int32.Parse(date[1]) - 1];
				year = date[2];

#else

				day = date[1];
				Debug.WriteLine("day: " + date[1]);
				Debug.WriteLine("month: " + date[0]);
				month = months[int.Parse(date[0]) - 1];
				year = date[2];

#endif
			}
			else
			{
				char[] d_delims = { '-' };
				string[] date = datetime[0].Split(d_delims);

				day = date[2];
				month = months[int.Parse(date[1]) - 1];
				year = date[0];
			}

			char[] t_delims = { ':' };
			string[] time = datetime[1].Split(t_delims);

			hour = (int.Parse(time[0]) % 12).ToString();
			afterMid = int.Parse(time[0]) / 12;

			if (int.Parse(hour) == 0 && afterMid == 1)
			{
				hour = "12";
			}

			minutes = time[1];

			switch (int.Parse(day))
			{
				case (1):
				case (21):
				case (31):
					date_suffix = "st";
					break;
				case (2):
				case (22):
					date_suffix = "nd";
					break;
				case (3):
				case (23):
					date_suffix = "rd";
					break;
				default:
					date_suffix = "th";
					break;
			}
			var time_suffix = string.Empty;
			switch (afterMid)
			{
				case (0):
					time_suffix = "am";
					break;
				case (1):
					time_suffix = "pm";
					break;
				default:
					time_suffix = "";
					break;
			}

			day = day.TrimStart(new char[] { '0' });

			displayString = string.Format("{0} {1}{2}, {3}", month, day, date_suffix, year);
			conversation.displayDate = displayString;

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
		/// Adds the ticket to displayed conversation.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public void addTicketToDisplayedConversation(Conversation conversation, Ticket ticket)
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
		public int[] parseDateToIntegers(Conversation conversation)
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

