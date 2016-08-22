//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using Xamarin.Forms;

//namespace TicketToTalk
//{
//	/// <summary>
//	/// Edit tags.
//	/// </summary>
//	public partial class EditTags : ContentPage
//	{

//		Entry newTag { get; set; }
//		Button addTag;
//		public List<Tag> selectTags { get; set; }
//		Ticket ticket;
//		List<Tag> relations = new List<Tag>();

//		StackLayout toggles;

//		/// <summary>
//		/// Initializes a new instance of the page.
//		/// </summary>
//		/// <param name="ticket">Ticket.</param>
//		public EditTags(Ticket ticket)
//		{
//			selectTags = new List<Tag>();
//			this.ticket = ticket;
//			Padding = new Thickness(20);
//			Title = "Edit Tags";

//			Console.WriteLine("Editing tags");
//			// Get all related tags.
//			TicketTagDB ticketTagDB = new TicketTagDB();
//			ticketTagDB.open();
//			var ticketRelations = ticketTagDB.getRelationByTicketID(ticket.id);

//			//TagDB tagDB = new TagDB();
//			var tagController = new TagController();
//			if (ticketRelations != null) 
//			{
//				foreach (TicketTag ttr in ticketRelations)
//				{
//					//Tag t = tagDB.GetTag(ttr.tag_id);
//					var t = tagController.getTag(ttr.tag_id);
//					relations.Add(t);
//				}
//			}
//			//ticketTagDB.close();

//			// Add button to navigation bar.
//			ToolbarItems.Clear();
//			ToolbarItems.Add(new ToolbarItem
//			{
//				Text = "Done",
//				Order = ToolbarItemOrder.Primary,
//				Command = new Command(saveChanges)
//			});

//			// Build a view of all tags with a toggle to add and delete relationships.
//			toggles = new StackLayout
//			{
//				Spacing = 5
//			};

//			// For all tags...
//			foreach (Tag t in tagController.getTags()) 
//			{
//				// Create a new layout
//				var cell = new StackLayout 
//				{
//					Orientation = StackOrientation.Horizontal,
//				};

//				// Add tag's text.
//				var text = new Label
//				{
//					Text = t.text,
//					HorizontalOptions = LayoutOptions.FillAndExpand
//				};

//				// Flush toggle to the right.
//				var toggle = new ToggleSwitch
//				{
//					HorizontalOptions = LayoutOptions.End,
//					tag = t
//				};

//				// Add toggle event.
//				toggle.Toggled += tagToggleEvent;

//				// If relationship between ticket and tag exists, set toggle in on position.
//				foreach (Tag previousTag in relations) 
//				{
//					if (t.Equals(previousTag)) 
//					{
//						toggle.IsToggled = true;
//						selectTags.Add(t);
//					}
//				}

//				// Add cell stack layout.
//				cell.Children.Add(text);
//				cell.Children.Add(toggle);
//				toggles.Children.Add(cell);
//			}

//			//tagDB.close();

//			Label addTagLabel = new Label 
//			{
//				Text = "Add a Tag"
//			};

//			newTag = new Entry 
//			{
//				Placeholder = "Add a Tag"
//			};
//			newTag.TextChanged += EnableSaveButton;

//			addTag = new Button
//			{
//				Text = "Save Tag",
//				IsEnabled = false
//			};
//			addTag.Clicked += saveTag;

//			Content = new StackLayout
//			{
//				Spacing = 12,
//				Children = 
//				{
//					addTagLabel,
//					newTag,
//					addTag,
//					toggles
//				}
//			};
//		}

		/// <summary>
		/// Initializes a new instance of the page.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
//		public EditTags(List<Tag> in_tags)
//		{
//			this.selectTags = in_tags;
//			this.BindingContext = selectTags;
//			//selectTags = new List<Tag>();
//			Padding = new Thickness(20);
//			Title = "Edit Tags";

//			// Add button to navigation bar.
//			ToolbarItems.Clear();
//			ToolbarItems.Add(new ToolbarItem
//			{
//				Text = "Done",
//				Order = ToolbarItemOrder.Primary,
//				Command = new Command(saveSelection)
//			});

//			// Build a view of all tags with a toggle to add and delete relationships.
//			toggles = new StackLayout
//			{
//				Spacing = 5
//			};

//			TagDB tagDB = new TagDB();
//			tagDB.open();
//			// For all tags...
//			foreach (Tag t in tagDB.GetTags())
//			{
//				// Create a new layout
//				var cell = new StackLayout
//				{
//					Orientation = StackOrientation.Horizontal,
//				};

//				// Add tag's text.
//				var text = new Label
//				{
//					Text = t.text,
//					HorizontalOptions = LayoutOptions.FillAndExpand
//				};

//				// Flush toggle to the right.
//				var toggle = new ToggleSwitch
//				{
//					HorizontalOptions = LayoutOptions.End,
//					tag = t
//				};

//				// Add toggle event.
//				toggle.IsToggled = false;
//				toggle.Toggled += tagToggleEvent;

//				// Add cell stack layout.
//				cell.Children.Add(text);
//				cell.Children.Add(toggle);
//				toggles.Children.Add(cell);
//			}
//			tagDB.close();

//			Label addTagLabel = new Label
//			{
//				Text = "Add a Tag"
//			};

//			newTag = new Entry
//			{
//				Placeholder = "Add a Tag"
//			};
//			newTag.TextChanged += EnableSaveButton;

//			addTag = new Button
//			{
//				Text = "Save Tag",
//				IsEnabled = false
//			};
//			addTag.Clicked += saveTag;

//			var finishButton = new Button 
//			{
//				Text = "Done"
//			};
//			finishButton.Clicked += FinishButton_Clicked;

//			Content = new StackLayout
//			{
//				Spacing = 12,
//				Children =
//				{
//					addTagLabel,
//					newTag,
//					addTag,
//					toggles,
//					finishButton
//				}
//			};
//		}

//		/// <summary>
//		/// On text change on tag entry, the 'save' tag button becomes available.
//		/// </summary>
//		/// <returns>void</returns>
//		/// <param name="sender">Sender: Entry</param>
//		/// <param name="ea">Ea: Environment args</param>
//		public void EnableSaveButton(object sender, EventArgs ea)
//		{
//			addTag.IsEnabled = true;
//		}

//		/// <summary>
//		/// Saves the tag.
//		/// </summary>
//		/// <returns>The tag.</returns>
//		/// <param name="sender">Sender: Button</param>
//		/// <param name="ea">Ea: Environment args</param>
//		// TODO: Use controller method.
//		public async void saveTag(object sender, EventArgs ea)
//		{
//			Console.WriteLine("Saving tag");

//			IDictionary<string, string> parameters = new Dictionary<string, string>();
//			parameters["text"] = newTag.Text;
//			parameters["token"] = Session.Token.val;

//			NetworkController net = new NetworkController();
//			var jobject = await net.sendPostRequest("tags/store", parameters);
//			var jtoken = jobject.GetValue("tag");
//			var tag = jtoken.ToObject<Tag>();

//			Console.WriteLine(tag);

//			// Check tag already exists.
//			TagDB tagDB = new TagDB();
//			var stored = tagDB.GetTag(tag.id);

//			if (stored == null) 
//			{
//				tagDB.AddTag(tag);
//				// add to stack.
//				var cell = new StackLayout
//				{
//					Orientation = StackOrientation.Horizontal,
//				};

//				var text = new Label
//				{
//					Text = tag.text,
//					HorizontalOptions = LayoutOptions.FillAndExpand
//				};

//				var toggle = new ToggleSwitch
//				{
//					HorizontalOptions = LayoutOptions.End,
//					tag = tag
//				};
//				toggle.Toggled += tagToggleEvent;

//				cell.Children.Add(text);
//				cell.Children.Add(toggle);
//				toggles.Children.Add(cell);
//			}
//			tagDB.close();
//		}

//		/// <summary>
//		/// Saves the changes.
//		/// </summary>
//		/// <returns>The changes.</returns>
//		public void saveChanges() 
//		{
//			TicketTagDB ticketTagDB = new TicketTagDB();
//			ticketTagDB.open();
//			// Remove all current related tags.
//			var relation = ticketTagDB.getRelationByTicketID(ticket.id);
//			if (relation != null) 
//			{
//				foreach (TicketTag ttr in relation)
//				{
//					ticketTagDB.DeleteRelation(ttr.id);
//				}
//			}

//			foreach (Tag t in selectTags) 
//			{
//				TicketTag ttr = new TicketTag(ticket.id, t.id);
//				ticketTagDB.AddTicketTagRelationship(ttr);
//			}
//			ticketTagDB.close();
//			Navigation.PopAsync();
//		}

//		//public void saveSelection() 
//		//{
//		//	foreach (Tag t in selectTags) 
//		//	{
//		//		NewTicket.tagsToAdd.Add(t);
//		//		Debug.WriteLine("tagsToAdd Length" + NewTicket.tagsToAdd.Count);
//		//	}
//		//	Navigation.PopAsync();
//		//}

//		/// <summary>
//		/// Event delegate for a toggled tag.
//		/// If the tag is toggled to an on position, add the relationship, else, remove the relationship.
//		/// </summary>
//		/// <returns>The toggle event.</returns>
//		/// <param name="sender">Sender.</param>
//		/// <param name="ea">Ea.</param>
//		public void tagToggleEvent(Object sender, EventArgs ea) 
//		{
//			var toggle = (ToggleSwitch)sender;
//			Tag t = toggle.tag;

//			if (toggle.IsToggled)
//			{
//				selectTags.Add(t);
//				Debug.WriteLine("Adding tag to selectTags: " + t);
//			}
//			else 
//			{
//				selectTags.Remove(t);
//			}
//		}

//		public void FinishButton_Clicked(object sender, EventArgs e)
//		{
//			Console.WriteLine("NUM TAGS SELECTED FROM FORM: " + selectTags.Count);
//			MessagingCenter.Send<EditTags, List<Tag>>(this, "selected_tags", selectTags);
//			Navigation.PopModalAsync();
//		}
//	}
//}