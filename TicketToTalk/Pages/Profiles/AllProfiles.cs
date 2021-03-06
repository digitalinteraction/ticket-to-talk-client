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
	public class AllProfiles : TrackedContentPage
	{
		public static bool promptShown = false;
		public static ObservableCollection<Person> people = new ObservableCollection<Person>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.AllProfiles"/> class.
		/// </summary>
		public AllProfiles()
		{

            TrackedName = "All Profiles";

			people.Clear();
			this.Title = "Profiles";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(launchAddNewPersonView)
			});

			var personController = new PersonController();

			var task = Task.Run(() => personController.GetPeopleFromServer());

			try
			{
				people = task.Result;
			}
			catch (Exception ex)
			{
				people = new ObservableCollection<Person>(personController.GetPeople());
				Debug.WriteLine(ex.StackTrace);
			}

			var tableView = new TableView
			{
				Intent = TableIntent.Form,
				Root = new TableRoot(),
				HasUnevenRows = true,
				RowHeight = 90
			};

			var userSection = new TableSection("Your Profile");

			var userCell = new UserCell();
			userCell.BindingContext = Session.activeUser;
			userCell.Tapped += UserCell_Tapped;
			userSection.Add(userCell);
			tableView.Root.Add(userSection);

			var tableSection = new TableSection("Your People");
			foreach (Person p in people)
			{
				var stored_person = personController.GetPerson(p.id);
				if (stored_person != null)
				{
					p.pathToPhoto = stored_person.pathToPhoto;
				}
				p.imageSource = Task.Run(() => personController.GetPersonProfilePicture(p)).Result;

				var personCell = new PersonCell(p);

				personCell.BindingContext = p;
				personCell.Tapped += PersonCell_Tapped;
				tableSection.Add(personCell);
			}


			if (people.Count != 0)
			{
				tableView.Root.Add(tableSection);
			}

			var stack = new StackLayout();
			stack.Children.Add(tableView);
			Content = stack;
        }

		/// <summary>
		/// Launch person profile on cell tap.
		/// </summary>
		/// <returns>The cell tapped.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void PersonCell_Tapped(object sender, EventArgs e)
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
		private void UserCell_Tapped(object sender, EventArgs e)
		{
			Navigation.PushAsync(new UserProfile());
		}

		/// <summary>
		/// Launchs the add new person view.
		/// </summary>
		/// <returns>The add new person view.</returns>
		private void launchAddNewPersonView()
		{
            Console.WriteLine("Adding new person");
			Navigation.PushAsync(new AddPersonChoice());
		}

		/// <summary>
		/// Ons the appearing.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Session.activeUser.firstLogin && !promptShown)
			{
				var canSkip = true;
				if (people.Count == 0)
				{
					canSkip = false;
				}
				var t = new AddNewPersonPrompt(canSkip);
				Application.Current.MainPage = t;
				promptShown = true;
			}
		}
	}
}