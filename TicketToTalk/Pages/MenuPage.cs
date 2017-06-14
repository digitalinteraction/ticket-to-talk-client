using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Menu page.
	/// </summary>
	public class MenuPage : TrackedContentPage
	{
		public ListView Menu { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.MenuPage"/> class.
		/// </summary>
		public MenuPage()
		{

            TrackedName = "Menu";

			NavigationPage.SetHasNavigationBar(this, false);
			Icon = "menu_icon.png";
			Title = "Menu"; // The Title property must be set.
			BackgroundColor = ProjectResource.color_white;

			Menu = new MenuListView();

			var userName = new Label
			{
			};
            userName.SetSubHeaderStyle();
            userName.VerticalOptions = LayoutOptions.Center;
            userName.TextColor = ProjectResource.color_white;
			userName.SetBinding(Label.TextProperty, "name");
			userName.BindingContext = Session.activeUser;

            var userEmail = new Label
            {
                Text = Session.activeUser.email,
            };
            userEmail.SetBodyStyle();
            userEmail.VerticalOptions = LayoutOptions.Center;
            userEmail.TextColor = ProjectResource.color_white;

			var menuContent = new ContentView
			{
                VerticalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_blue,
				Content = new StackLayout 
				{
					Children = 
					{
						userName,
                        userEmail
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
				Padding = new Thickness(0, 20, 0, 0),
				BackgroundColor = ProjectResource.color_blue,
				Spacing = 0,
				Orientation = StackOrientation.Horizontal,
				Children = 
				{
					new UserProfileImage((Session.ScreenWidth * 0.2), "left", ProjectResource.color_white),
                    menuContent
				}
			};

			var layout = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					images,
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