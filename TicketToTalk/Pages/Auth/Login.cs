using System;
using System.Diagnostics;
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
		private Button login;
		private Button register;
		private ActivityIndicator indicator = null;
		ScrollView scrollView;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Login"/> class.
		/// </summary>
		public Login()
		{

			BackgroundColor = ProjectResource.color_blue;

			NavigationPage.SetHasNavigationBar(this, false);

			indicator = new ProgressSpinner(this, ProjectResource.color_grey_transparent);

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
					register,
				}
			};

			var layout = new AbsoluteLayout();

			scrollView = new ScrollView 
			{
				Content = stack
			};

			AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0.5, 0.5, Session.ScreenHeight, Session.ScreenWidth));
			AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);

			layout.Children.Add(scrollView);
			layout.Children.Add(indicator);

			Content = layout;
		}

		/// <summary>
		/// Event handler for login button press.
		/// </summary>
		/// <returns>The login.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="ea">Ea.</param>
		private async void HandleLogin(object sender, EventArgs ea)
		{
			await register.FadeTo(0, 500);
			await login.FadeTo(0, 500);
			await password.FadeTo(0, 500);
			await email.FadeTo(0, 500);

			IsBusy = true;

			Debug.WriteLine(IsBusy);

			var userController = new UserController();

			bool authed = false;

			try
			{
				authed = await userController.AuthenticateUser(email.Text, password.Text);

				Debug.WriteLine(indicator.IsRunning);
				Debug.WriteLine(indicator.IsVisible);

				if (authed)
				{
					Session.activeUser.imageSource = await userController.GetUserProfilePicture();

					var v = short.Parse(Session.activeUser.verified);
					if (v > 0)
					{
						
						IsBusy = false;

						await Navigation.PushAsync(new SelectActivePerson());
						Navigation.RemovePage(this);
					}
					else
					{
						IsBusy = false;

						await Navigation.PushAsync(new Verification(false));
						Navigation.RemovePage(this);
					}
				}
				else
				{
					IsBusy = false;

					await DisplayAlert("Login", "Incorrect email or password", "OK");

					await email.FadeTo(1, 500);
					await password.FadeTo(1, 500);
					await login.FadeTo(1, 500);
					await register.FadeTo(1, 500);
				}
			}
			catch (NoNetworkException ex)
			{
				IsBusy = false;

				await DisplayAlert("No Network", ex.Message, "Dismiss");

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
		private async void HandleRegister(object sender, EventArgs ea)
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
		private void Entry_TextChanged(object sender, EventArgs e)
		{
			if (sender == email) 
			{
				email.Focus();
				//scrollView.ScrollToAsync(email, ScrollToPosition.End, true);
			}
			if (sender == password)
			{
				password.Focus();
				//scrollView.ScrollToAsync(password, ScrollToPosition.End, true);
			}
			var entriesNotNull = (!string.IsNullOrEmpty(email.Text))
				&& (!string.IsNullOrEmpty(password.Text));

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

