using System;
namespace TicketToTalk
{
	/// <summary>
	/// Conversation item.
	/// </summary>
	public class ConversationItem
	{

		public Conversation conversation { get; set;}
		public Ticket ticket { get; set; }
		public string title { get; set; }
		public string year { get; set; }
		public string displayIcon { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationItem"/> class.
		/// </summary>
		public ConversationItem(Conversation conversation, Ticket ticket)
		{
			this.conversation = conversation;
			this.ticket = ticket;
			title = ticket.title;
			year = ticket.year;
			displayIcon = ticket.displayIcon;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.ConversationItem"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.ConversationItem"/>.</returns>
		public override string ToString()
		{
			return string.Format("[ConversationItem: conversation={0}, ticket={1}, title={2}, year={3}, displayIcon={4}]", conversation, ticket, title, year, displayIcon);
		}
	}
}

