using System;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class ConversationCell : ViewCell
	{
		public ConversationCell()
		{
			var deleteCell = new MenuItem
			{
				Text = "Delete",
				IsDestructive = true
			};
			deleteCell.Clicked += DeleteCell_Clicked;

			ContextActions.Add(deleteCell);

			var dayMonth = new Label
			{
				FontSize = 18,
				TextColor = ProjectResource.color_dark,
			};
			dayMonth.SetBinding(Label.TextProperty, "displayDate");

			var ticketCount = new Label
			{
				FontSize = 12,
				TextColor = ProjectResource.color_red,
			};
			ticketCount.SetBinding(Label.TextProperty, new Binding("ticketCount", stringFormat: "Ticket(s): {0}"));

			var content = new StackLayout()
			{
				Spacing = 5,
				Padding = new Thickness(10, 5, 10, 5),
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					dayMonth,
					ticketCount
				}
			};

			this.View = content;
		}

		/// <summary>
		/// Deletes the cell clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void DeleteCell_Clicked(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);

			var conversation = (Conversation)mi.BindingContext;

			var conversationController = new ConversationController();
			conversationController.DestroyConversation(conversation);
		}
	}
}

