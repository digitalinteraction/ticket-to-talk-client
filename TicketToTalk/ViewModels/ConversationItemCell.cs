using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class ConversationItemCell : ImageCell
	{
		public ConversationItemCell()
		{
			var deleteCell = new MenuItem
			{
				Text = "Remove",
				IsDestructive = true
			};
			deleteCell.Clicked += DeleteCell_Clicked;

			ContextActions.Add(deleteCell);
		}

		/// <summary>
		/// Deletes the cell clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void DeleteCell_Clicked(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
			Debug.WriteLine("TicketCell: Binding context - " + mi.BindingContext);

			var conversationItem = (ConversationItem)mi.BindingContext;

			var conversationController = new ConversationController();
			var removed = await conversationController.removeTicketFromConversationRemotely(conversationItem.conversation, conversationItem.ticket);

			if (removed) 
			{
				conversationController.removeTicketFromConversation(conversationItem.conversation, conversationItem.ticket);
				ConversationView.conversationItems.Remove(conversationItem);
				ConversationSelect.conversations.Remove(conversationItem.conversation);
			}
		}
	}
}

