﻿using System;
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
		public void StoreConversationLocally(Conversation conversation)
		{
			convDB.Open();
			convDB.AddConversation(conversation);
			convDB.Close();
		}

		/// <summary>
		/// Gets the conversation.
		/// </summary>
		/// <returns>The conversation.</returns>
		/// <param name="id">Identifier.</param>
		public Conversation GetConversationLocally(int id)
		{
			convDB.Open();
			var conversation = convDB.GetConversation(id);
			convDB.Close();

			return conversation;
		}

		/// <summary>
		/// Gets the conversations.
		/// </summary>
		/// <returns>The conversations.</returns>
		public List<Conversation> GetConversationsRemotely()
		{
			convDB.Open();
			var convs = new List<Conversation>(convDB.GetConversationsForPerson());
			convDB.Close();

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
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["datetime"] = conversation.date;
			parameters["ticket_id_string"] = conversation.ticket_id_string;
			parameters["platform"] = platform;
			parameters["notes"] = conversation.notes;
			parameters["person_id"] = conversation.person_id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.SendPostRequest("conversations/store", parameters);

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
		public async Task<Conversation> UpdateConversationRemotely(Conversation conversation)
		{
			// Build the parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["notes"] = conversation.notes;
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.SendPostRequest("conversations/update", parameters);

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
		public void UpdateConversationLocally(Conversation conversation)
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
			convDB.Open();
			convDB.DeleteConversation(conversation.id);
			convDB.AddConversation(conversation);
			convDB.Close();
		}

		/// <summary>
		/// Deletes the conversation locally.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public void DeleteConversationLocally(Conversation conversation)
		{
			convDB.Open();
			convDB.DeleteConversation(conversation.id);
			convDB.Close();
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
			var jobject = await networkController.SendGetRequest("conversations/destroy", parameters);

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
		public async void DestroyConversation(Conversation conversation)
		{
			// Delete conversation remotely.
			var deleted = await DeleteConversationRemotely(conversation);

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
		public async Task<List<Conversation>> GetRemoteConversations()
		{
			// Build paramters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = Session.activePerson.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.SendGetRequest("conversations/get", parameters);

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
		public Conversation AddTicketToConversation(Conversation conversation, Ticket ticket)
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
				temp = string.Format("{0} ", s);
			}

			conversation.ticket_id_string = temp.TrimEnd();
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
			return null;
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
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.SendPostRequest("conversations/tickets/add", parameters);

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
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["conversation_id"] = conversation.id.ToString();
			parameters["ticket_id"] = ticket.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send request.
			var jobject = await networkController.SendPostRequest("conversations/tickets/remove", parameters);

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

