using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Menu page.
	/// </summary>
	public class MenuPage : ContentPage
	{
		public ListView Menu { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.MenuPage"/> class.
		/// </summary>
		public MenuPage()
		{
			NavigationPage.SetHasNavigationBar(this, false);
			Icon = "menu_icon.png";
			Title = "Menu"; // The Title property must be set.
			BackgroundColor = ProjectResource.color_blue;

			Padding = new Thickness(0, 20, 0, 0);

			Menu = new MenuListView();

			var menuContent = new ContentView
			{
				Padding = new Thickness(20, 0, 0, 20),
				Content = new StackLayout 
				{
					Children = 
					{
						new Label
						{
							Text = Session.activeUser.name,
							FontSize = 14,
							TextColor = ProjectResource.color_white,
							FontAttributes = FontAttributes.Bold
						},
						new Label
						{
							Text = Session.activeUser.email,
							FontSize = 14,
							TextColor = ProjectResource.color_white,
						}
					}
				},
			};

			PersonProfileImage person = new PersonProfileImage(Session.activePerson);
			person.profilePic.WidthRequest = (Session.ScreenWidth * 0.1);
			person.profilePic.HeightRequest = (Session.ScreenWidth * 0.1);
			person.profilePic.BorderColor = ProjectResource.color_white;
			person.profilePic.VerticalOptions = LayoutOptions.StartAndExpand;
			person.profilePic.HorizontalOptions = LayoutOptions.EndAndExpand;

			var images = new StackLayout
			{
				Spacing = 0,
				Orientation = StackOrientation.Horizontal,
				Children = 
				{
					new UserProfileImage(Session.activeUser, (Session.ScreenWidth * 0.2), "left", ProjectResource.color_white),
				}
			};

			var layout = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					images,
					menuContent,
					new StackLayout
					{
						Padding = new Thickness(0,10,0,0),
						BackgroundColor = ProjectResource.color_white,
						Children = 
						{
							Menu
						}
					}
				}
			};

			Content = layout;
		}
	}
}