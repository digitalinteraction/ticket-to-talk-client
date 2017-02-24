using System;
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
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

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
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0, 0, 0, 10),
				IsEnabled = true
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
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewConversation"/> class.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public NewConversation(Conversation conversation)
		{
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			Title = "Edit Conversation";

			var dateLabel = new Label
			{
				Text = "Select a date for the conversation."
			};

			var dates = conversationController.ParseDateToIntegers(conversation);

			datepicker = new DatePicker
			{
				TextColor = ProjectResource.color_red,
			};
			datepicker.Date = new DateTime(dates[2], dates[1], dates[0]);

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
			timePicker.Time = new TimeSpan(dates[3], dates[4], dates[5]);

			var notesLabel = new Label
			{
				Text = "Notes",
				Margin = new Thickness(0, 10, 0, 0)
			};

			notes = new Editor()
			{
				Text = conversation.notes,
				TextColor = ProjectResource.color_red
			};

			saveButton = new Button()
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0, 0, 0, 10),
				IsEnabled = true
			};
			//saveButton.Clicked += SaveButton_Clicked;

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
		/// Cancel this instance.
		/// </summary>
		private void Cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// Entries the changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void EntryChanged(object sender, EventArgs e)
		{
			//var notNull = datepicker.Date != null
			//	&& timePicker.Time != null
			var notNull = (!string.IsNullOrEmpty(notes.Text));

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
		private async void SaveButton_Clicked(object sender, EventArgs e)
		{
			saveButton.IsEnabled = false;
			var dateTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", datepicker.Date.Year, datepicker.Date.Month, datepicker.Date.Day, timePicker.Time.Hours, timePicker.Time.Minutes, timePicker.Time.Seconds);

			var conversation = new Conversation();
			conversation.person_id = Session.activePerson.id;
			conversation.notes = notes.Text;
			conversation.date = dateTime;

			var returned = await conversationController.StoreConversationRemotely(conversation);
			if (returned != null)
			{
				conversationController.StoreConversationLocally(returned);
				ConversationsView.conversations.Add(conversationController.SetPropertiesForDisplay(returned));
				ConversationSelect.conversations.Add(conversationController.SetPropertiesForDisplay(returned));
			}
			else
			{
				await DisplayAlert("New Conversation", "Conversation could not be added.", "OK");
				saveButton.IsEnabled = true;
			}

			await Navigation.PopModalAsync();
		}
	}
}

