using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Title page.
	/// Handles loggin in and launches user registration.
	/// </summary>
	public partial class Login : ContentPage
	{
		private Label title;
		private Label loadTime;
		private Entry email;
		private Entry password;
		Button login;
		Button register;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Login"/> class.
		/// </summary>
		public Login()
		{

			BackgroundColor = ProjectResource.color_blue;

			NavigationPage.SetHasNavigationBar(this, false);

			Title = "Title";

			Padding = new Thickness(20);

			title = new Label
			{
				Text = "Ticket to Talk",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontSize = 42,
				TextColor = ProjectResource.color_white,
				HorizontalTextAlignment = TextAlignment.Center
			};

			loadTime = new Label
			{
				Text = "Logging in",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_blue
			};

			login = new Button
			{
				Text = "Login",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				WidthRequest = (Session.ScreenWidth * 0.5),
				IsEnabled = false
			};
			login.Clicked += HandleLogin;

			email = new Entry
			{
				Placeholder = "Email address",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_blue,
				TextColor = ProjectResource.color_white,
				PlaceholderColor = ProjectResource.color_dark,
				WidthRequest = (Session.ScreenWidth * 0.75)
			};
			email.TextChanged += Entry_TextChanged;

			password = new Entry
			{
				Placeholder = "Password",
				IsPassword = true,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_white,
				PlaceholderColor = ProjectResource.color_dark,
				WidthRequest = (Session.ScreenWidth * 0.75)
			};
			password.TextChanged += Entry_TextChanged;

			register = new Button 
			{
				Text = "Register",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_dark,
				WidthRequest = (Session.ScreenWidth * 0.5)
			};
			register.Clicked += HandleRegister;

			var stack = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 12,
				Children = 
				{
					title, 
					loadTime, 
					email, 
					password, 
					login, 
					register
				}
			};

			Content = new ScrollView
			{
				Content = stack
			};
		}

		/// <summary>
		/// Event handler for login button press.
		/// </summary>
		/// <returns>The login.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="ea">Ea.</param>
		async void HandleLogin(Object sender, EventArgs ea)
		{
			await register.FadeTo(0, 500);
			await login.FadeTo(0, 500);
			await password.FadeTo(0, 500);
			await email.FadeTo(0, 500);

			var userController = new UserController();
			var authed = await userController.authenticateUser(email.Text, password.Text);
			if (authed)
			{
				await Navigation.PushAsync(new SelectActivePerson());
				Navigation.RemovePage(this);
			}
			else 
			{
				await DisplayAlert("Login", "Incorrect email or password", "OK");

				await email.FadeTo(1, 500);
				await password.FadeTo(1, 500);
				await login.FadeTo(1, 500);
				await register.FadeTo(1, 500);
			}
		}

		/// <summary>
		/// Event handler for registration button pressed.
		/// </summary>
		/// <returns>The register.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="ea">Ea.</param>
		async void HandleRegister(Object sender, EventArgs ea) 
		{
			await register.FadeTo(0, 500);
			await login.FadeTo(0, 500);
			await password.FadeTo(0, 500);
			await email.FadeTo(0, 500);

			var nav = new Register();
			await Navigation.PushAsync(nav);
			Navigation.RemovePage(this);
		}

		/// <summary>
		/// On Entry text change.
		/// </summary>
		/// <returns>The text changed.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Entry_TextChanged(object sender, EventArgs e)
		{
			var entriesNotNull = (!String.IsNullOrEmpty(email.Text))
				&& (!String.IsNullOrEmpty(password.Text));

			if (entriesNotNull)
			{
				login.BackgroundColor = ProjectResource.color_red;
				login.IsEnabled = true;
			}
			else
			{
				login.BackgroundColor = ProjectResource.color_grey;
				login.IsEnabled = false;
			}
		}
	}
}

