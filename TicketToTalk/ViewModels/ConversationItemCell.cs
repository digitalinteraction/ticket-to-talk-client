using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Conversation item cell.
	/// </summary>
    public class ConversationItemCell : ViewCell
	{
        private Image ticketIcon;
        private Label titleLabel;
        private Label year;

        public ConversationItemCell()
		{
			var deleteCell = new MenuItem
			{
				Text = "Remove",
				IsDestructive = true
			};
			deleteCell.Clicked += DeleteCell_Clicked;

			ContextActions.Add(deleteCell);

			ticketIcon = new Image
			{
				HeightRequest = 50,
				WidthRequest = 50,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			ticketIcon.SetBinding(Image.SourceProperty, "imageSource");

			titleLabel = new Label
			{
			};
			titleLabel.SetSubHeaderStyle();
			titleLabel.VerticalOptions = LayoutOptions.Start;
			titleLabel.SetBinding(Label.TextProperty, "title");

			year = new Label
			{
			};
			year.SetBodyStyle();
			year.TextColor = ProjectResource.color_red;
			year.VerticalOptions = LayoutOptions.Start;
			year.SetBinding(Label.TextProperty, "year");

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(10, 0, 0, 0),
				Spacing = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					titleLabel,
					year
				}
			};

			var cellLayout = new StackLayout
			{
				Spacing = 0,
				Padding = new Thickness(10, 5, 10, 5),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					ticketIcon,
					detailsStack
				}
			};
			this.View = cellLayout;
		}

		/// <summary>
		/// Deletes the cell clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void DeleteCell_Clicked(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);

			var conversationItem = (ConversationItem)mi.BindingContext;

			var conversationController = new ConversationController();

			bool removed = false;

			try
			{
				removed = await conversationController.RemoveTicketFromConversationRemotely(conversationItem.conversation, conversationItem.ticket);
			}
			catch (NoNetworkException ex)
			{
				await Application.Current.MainPage.DisplayAlert("No Network", ex.Message, "Dismiss");
			}

			if (removed)
			{
				conversationController.RemoveTicketFromConversation(conversationItem.conversation, conversationItem.ticket);
				ConversationView.conversationItems.Remove(conversationItem);
				ConversationSelect.conversations.Remove(conversationItem.conversation);

				foreach (Conversation c in ConversationsView.conversations)
				{
					if (c.id == conversationItem.conversation.id)
					{
						c.ticketCount--;
						break;
					}
				}
			}
		}
	}
}

