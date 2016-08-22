using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ticket_to_Talk
{
	public class Profiles : ContentPage
	{
		List<Person> people;
		public Profiles()
		{
			Title = "Profiles";

			var userDB = new UserDB();
			var user = userDB.getUserByEmail(Session.email);
			userDB.close();

			//getPeople();

			var tableView = new TableView
			{
				Intent = TableIntent.Form,
				Root = new TableRoot
				{
					new TableSection
					{
						new ImageCell
						{
							ImageSource = user.pathToPhoto,
							Text = user.name,
							Detail = user.email
						}
					}
				}
			};

			Content = new StackLayout
			{
				Children = {
					tableView
				}
			};
		}

		public async void getPeople()
		{
			NetworkController net = new NetworkController();
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;
			var response = await net.sendGetRequest("user/getpeople", parameters);

			var person = response.GetValue("people");
			Console.WriteLine(person.ToString());
		}
	}
}


