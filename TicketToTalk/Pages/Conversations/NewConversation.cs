using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// New conversation.
	/// </summary>
	public class NewConversation : TrackedContentPage
	{
		private DatePicker datepicker;
		private Editor notes;
		private Button saveButton;
		private TimePicker timePicker;

		private ConversationController conversationController = new ConversationController();
		Conversation conversation;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewConversation"/> class.
		/// </summary>
		public NewConversation()
		{

            TrackedName = "New Conversation";

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
            dateLabel.SetSubHeaderStyle();

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
            timeLabel.SetSubHeaderStyle();

			timePicker = new TimePicker
			{
				TextColor = ProjectResource.color_red,
			};

			var notesLabel = new Label
			{
				Text = "Notes",
				Margin = new Thickness(0, 10, 0, 0)
			};
            notesLabel.SetSubHeaderStyle();

			notes = new Editor()
			{
				Text = "Add some notes about the conversation..."
			};
            notes.SetStyle();
            notes.TextColor = ProjectResource.color_red;

			saveButton = new Button()
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				IsEnabled = true
			};
            saveButton.SetStyle();
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

			var stackLayout = new StackLayout
			{
				Spacing = 0,
				Children =
				{
					content,
					buttonStack

				}
			};

			var stack = new ScrollView
			{
                Content = stackLayout
            };

            Content = stack;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewConversation"/> class.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public NewConversation(Conversation conversation)
		{
			this.conversation = conversation;

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

			//var dates = conversationController.ParseDateToIntegers(conversation);

			datepicker = new DatePicker
			{
				TextColor = ProjectResource.color_red,
			};
			datepicker.Date = new DateTime(conversation.timestamp.Year, conversation.timestamp.Month, conversation.timestamp.Day);

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
			timePicker.Time = new TimeSpan(conversation.timestamp.Hour, conversation.timestamp.Minute, conversation.timestamp.Second);

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
				Text = "Update",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0, 0, 0, 10),
				IsEnabled = true
			};
			//updateButton.Clicked += UpdateButton_Clicked;
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

			var stackLayout = new StackLayout
			{
				Spacing = 0,
				Children =
				{  
					content,
					buttonStack

				}
			};

			var stack = new ScrollView
			{
				Content = stackLayout
			};

            Content = stack;
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

			if (conversation != null)
			{
				conversation.person_id = Session.activePerson.id;
				conversation.notes = notes.Text;
				conversation.date = dateTime;
				conversation.timestamp = new DateTime(datepicker.Date.Year, datepicker.Date.Month, datepicker.Date.Day, timePicker.Time.Hours, timePicker.Time.Minutes, timePicker.Time.Seconds);

				var success = false;

				try
				{
					success	= await conversationController.UpdateConversationRemotely(conversation);

					if (!success)
					{
						await DisplayAlert("Edit Conversation", "Conversation could not be updated.", "OK");
						saveButton.IsEnabled = true;
					}

					await Navigation.PopModalAsync();
				}
				catch (NoNetworkException ex)
				{
					await DisplayAlert("No Network", ex.Message, "Dismiss");
					saveButton.IsEnabled = true;
				}
			}
			else 
			{
				var o_conv = new Conversation();
				o_conv.person_id = Session.activePerson.id;
				o_conv.notes = notes.Text;
				o_conv.date = dateTime;

				Conversation returned = null;

				try
				{
					returned = await conversationController.StoreConversationRemotely(o_conv);

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
				catch (NoNetworkException ex)
				{
					await DisplayAlert("No Network", ex.Message, "Dismiss");
					saveButton.IsEnabled = true;
				}
			}
		}
	}
}
