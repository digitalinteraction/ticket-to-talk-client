// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 26/08/2016
//
// FinishConversation.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Finish conversation.
	/// </summary>
	public class FinishConversation : ContentPage
	{

		Conversation conversation;
		Editor editor;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.FinishConversation"/> class.
		/// </summary>
		public FinishConversation(Conversation conversation)
		{
			Padding = new Thickness(20);
			this.conversation = conversation;
			Title = "Finish Conversation";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			var label = new Label
			{
				Text = "How was your conversation? Update your notes below",
				TextColor = ProjectResource.color_dark
			};

			editor = new Editor
			{
				Text = conversation.notes,
				TextColor = ProjectResource.color_red
			};

			var button = new Button
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Margin = new Thickness(0, 0, 0, 10)
			};
			button.Clicked += Button_Clicked;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					label,
					editor,
					button,
				}
			};
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		private void cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// Update the conversation notes on press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void Button_Clicked(object sender, EventArgs e)
		{
			conversation.notes = editor.Text;
			var conversationController = new ConversationController();
			var updated = await conversationController.updateConversationRemotely(conversation);
			if (updated != null)
			{
				conversationController.updateConversationLocally(updated);
			}
			else
			{
				await DisplayAlert("Conversations", "Your notes could not be updated.", "OK");
			}
			//await Navigation.PopToRootAsync(true);
			await Navigation.PopModalAsync();
			//await Navigation.PopModalAsync();
		}
	}
}


