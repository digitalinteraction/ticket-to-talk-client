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
	public class FinishConversation : TrackedContentPage
	{

		private Conversation conversation;
		private Editor editor;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.FinishConversation"/> class.
		/// </summary>
		public FinishConversation(Conversation conversation)
		{

            TrackedName = "Finish Conversation";

			Padding = new Thickness(20);
			this.conversation = conversation;
			Title = "Finish Conversation";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			var label = new Label
			{
				Text = "How was your conversation? Update your notes below",
			};
            label.SetSubHeaderStyle();
            label.VerticalOptions = LayoutOptions.Start;

			editor = new Editor
			{
				Text = conversation.notes,
			};
            editor.SetStyle();
            editor.TextColor = ProjectResource.color_red;
            editor.VerticalOptions = LayoutOptions.Start;

			var button = new Button
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				VerticalOptions = LayoutOptions.EndAndExpand,
			};
            button.SetStyle();
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
		private void Cancel()
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
			var updated = await conversationController.UpdateConversationRemotely(conversation);
			if (updated)
			{
			}
			else
			{
				await DisplayAlert("Conversations", "Your notes could not be updated.", "OK");
			}

			await Navigation.PopModalAsync();
		}
	}
}
