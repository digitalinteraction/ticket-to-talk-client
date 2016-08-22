﻿using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// New conversation.
	/// </summary>
	public class NewConversation : ContentPage
	{
		private DatePicker datepicker;
		private Editor notes;
		private Button saveButton;
		private TimePicker timePicker;

		private ConversationController conversationController = new ConversationController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewConversation"/> class.
		/// </summary>
		public NewConversation()
		{

			Title = "New Conversation";

			var dateLabel = new Label 
			{
				Text = "Select a date for the conversation."
			};

			datepicker = new DatePicker
			{
				TextColor = ProjectResource.color_red,
			};
			datepicker.DateSelected += EntryChanged;

			var timeLabel = new Label
			{
				Text = "Select a time for the conversation.",
				Margin = new Thickness(0, 10, 0, 0)
			};

			timePicker = new TimePicker 
			{
				TextColor = ProjectResource.color_red,
			};

			var notesLabel = new Label 
			{
				Text = "Notes",
				Margin = new Thickness(0, 10, 0, 0)
			};

			notes = new Editor()
			{
				Text = "Add some notes about the conversation...",
				TextColor = ProjectResource.color_red
			};

			saveButton = new Button()
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0, 0, 0, 10),
				IsEnabled = false
			};
			saveButton.Clicked += SaveButton_Clicked;

			var buttonStack = new StackLayout 
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Children = 
				{
					saveButton
				}
			};

			var content = new StackLayout 
			{
				Spacing = 5,
				Padding = 20,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children = 
				{
					dateLabel,
					datepicker,
					timeLabel,
					timePicker,
					notesLabel,
					notes
				}
			};

			Content = new StackLayout 
			{
				Spacing = 0,
				Children = 
				{
					content, 
					buttonStack

				}
			};
		}

		/// <summary>
		/// Entries the changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void EntryChanged(object sender, EventArgs e)
		{
			var notNull = datepicker.Date != null
				&& timePicker.Time != null
				&& (!string.IsNullOrEmpty(notes.Text));

			if (notNull)
			{
				saveButton.BackgroundColor = ProjectResource.color_blue;
				saveButton.IsEnabled = true;
			}
			else 
			{
				saveButton.BackgroundColor = ProjectResource.color_grey;
				saveButton.IsEnabled = false;
			}
		}

		/// <summary>
		/// Saves the button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void SaveButton_Clicked(object sender, EventArgs e)
		{
			char[] delimiters = {' '};
			string[] dateSplit = datepicker.Date.ToString().Split(delimiters);

			Debug.WriteLine("NewConversation: date = " + datepicker.Date.ToString());
			Debug.WriteLine("NewConversation: time = " + timePicker.Time.ToString());

			var dateTime = String.Format("{0} {1}", dateSplit[0], timePicker.Time);

			Debug.WriteLine("NewConversation: datetime = " + dateTime);

			var conversation = new Conversation();
			conversation.person_id = Session.activePerson.id;
			conversation.notes = notes.Text;
			//conversation.date = DateTime.ParseExact(dateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			conversation.date = dateTime;
			Debug.WriteLine("NewConversation: Conversation = " + conversation);

			var returned = await conversationController.storeConversationRemotely(conversation);
			if (returned != null)
			{
				conversationController.storeConversationLocally(returned);
				ConversationsView.conversations.Add(conversationController.setPropertiesForDisplay(returned));
			}
			else 
			{
				await DisplayAlert("New Conversation", "Conversation could not be added.", "OK");
			}

			await Navigation.PopAsync();
		}
	}
}

