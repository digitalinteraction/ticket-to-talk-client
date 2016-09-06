﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View all profiles.
	/// </summary>
	public class AllProfiles : ContentPage
	{
		public static ObservableCollection<Person> people = new ObservableCollection<Person>();
		UserController userController = new UserController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.AllProfiles"/> class.
		/// </summary>
		public AllProfiles()
		{
			people.Clear();
			this.Title = "Profiles";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(launchAddNewPersonView)
			});

			var personController = new PersonController();
			people = Task.Run(() => personController.getPeopleFromServer()).Result;
			User user = Session.activeUser;

			var tableView = new TableView
			{
				Intent = TableIntent.Form,
				Root = new TableRoot(),
				HasUnevenRows = true,
				RowHeight = 90
			};

			Debug.WriteLine("AllProfiles: Adding user cell.");
			var userSection = new TableSection("Your Profile");
			var userCell = new UserCell
			{
				user = user,
			};
			if (!(user.pathToPhoto.StartsWith("storage", StringComparison.Ordinal)))
			{
				user.pathToPhoto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), user.pathToPhoto);
			}
			else 
			{
				userController.downloadUserProfilePicture(user);
				user.pathToPhoto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), user.pathToPhoto);
			}
			userCell.BindingContext = Session.activeUser;
			userCell.Tapped += UserCell_Tapped;
			userSection.Add(userCell);
			tableView.Root.Add(userSection);

			Debug.WriteLine("AllProfiles: Adding people cells");
			var tableSection = new TableSection("Your People");
			var personUserDB = new PersonUserDB();
			foreach (Person p in people)
			{
				if (!(p.pathToPhoto.StartsWith("storage", StringComparison.Ordinal))) 
				{
					p.pathToPhoto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), p.pathToPhoto);
				}
				p.relation = personUserDB.getRelationByUserAndPersonID(user.id, p.id).relationship;

				Debug.WriteLine("AllProfiles: Adding person: " + p);

				var personCell = new PersonCell(p);
		
				personCell.BindingContext = p;
				personCell.Tapped += PersonCell_Tapped;
				tableSection.Add(personCell);
			}

			if (people.Count != 0) 
			{
				tableView.Root.Add(tableSection);
			}

			Content = new StackLayout
			{
				Children = {
					tableView
				}
			};
		}

		/// <summary>
		/// Launch person profile on cell tap.
		/// </summary>
		/// <returns>The cell tapped.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void PersonCell_Tapped(object sender, EventArgs e)
		{
			PersonCell cell = (PersonCell)sender;

			foreach (Person p in people) 
			{
				if (p.id == cell.person.id) 
				{
					Navigation.PushAsync(new PersonProfile(p));
				}
			}
		}

		/// <summary>
		/// Launch user profile on cell tap.
		/// </summary>
		/// <returns>The cell tapped.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void UserCell_Tapped(object sender, EventArgs e)
		{
			Navigation.PushAsync(new UserProfile());
		}

		/// <summary>
		/// Launchs the add new person view.
		/// </summary>
		/// <returns>The add new person view.</returns>
		void launchAddNewPersonView() 
		{
			Navigation.PushAsync(new AddPersonChoice());
		}
	}
}