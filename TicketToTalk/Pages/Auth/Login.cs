using System;
using System.Diagnostics;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Title page.
	/// Handles loggin in and launches user registration.
	/// </summary>
	public partial class Login : LoadingPage
	{
		private Label title;
		private Label loadTime;
		private Entry email;
		private Entry password;
		private Button login;
		private Button register;
		ScrollView scrollView;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Login"/> class.
		/// </summary>
		public Login()
        {
            TrackedName = "Login";

			BackgroundColor = ProjectResource.color_blue;

			NavigationPage.SetHasNavigationBar(this, false);

			indicator = new ProgressSpinner(this, ProjectResource.color_blue_transparent, ProjectResource.color_white);

			Title = "Login";

			Padding = new Thickness(20);

			title = new Label
			{
				Text = "Ticket to Talk",
			};
            title.SetHeaderStyle();
            title.FontSize = ProjectResource.TextSize_H1;
            title.TextColor = ProjectResource.color_white;

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
            login.SetStyle();
			login.Clicked += HandleLogin;

			email = new Entry
			{
				Placeholder = "Email address",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_blue,
				TextColor = ProjectResource.color_white,
				PlaceholderColor = ProjectResource.color_dark,
				WidthRequest = (Session.ScreenWidth * 0.75),
                Keyboard = Keyboard.Email,
			};
			email.TextChanged += Entry_TextChanged;
            email.SetStyle();
            email.PlaceholderColor = ProjectResource.color_white;

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
            password.SetStyle();
            password.PlaceholderColor = ProjectResource.color_white;

			register = new Button
			{
				Text = "Register",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_dark,
				WidthRequest = (Session.ScreenWidth * 0.5)
			};
			register.Clicked += HandleRegister;
            register.SetStyle();

			var pageContent = new StackLayout
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
				Content = pageContent
			};

			AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0.5, 0.5, Session.ScreenHeight, Session.ScreenWidth));
			AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);

			layout.Children.Add(scrollView);
			layout.Children.Add(indicator);

			Content = layout;

			//this.stack.Content = scrollView.Content;
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

			var userController = new UserController();

			bool authed = false;

			try
			{
				authed = await userController.AuthenticateUser(email.Text, password.Text);

				if (authed)
				{
					Session.activeUser.imageSource = await userController.GetUserProfilePicture();

					var v = short.Parse(Session.activeUser.verified);
					if (v > 0)
					{

						var nav = new SelectActivePerson();
						//var ready = await nav.SetUpSelectActivePerson();

						//if (ready) 
						//{
						IsBusy = false;

                        updateLastLoggedIn(email.Text);

						await Navigation.PushAsync(nav);
						Navigation.RemovePage(this);
						//}
					}
					else
					{
						IsBusy = false;

                        updateLastLoggedIn(email.Text);

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
			catch (Exception ex)
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

            var nav = new Register(email.Text, password.Text);
			await Navigation.PushAsync(nav);
			//Navigation.RemovePage(this);
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
			}
			if (sender == password)
			{
				password.Focus();
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


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            email.Text = getLastLoggedIn();

			await email.FadeTo(1, 500);
			await password.FadeTo(1, 500);
			await login.FadeTo(1, 500);
			await register.FadeTo(1, 500);
        }

        /// <summary>
        /// Updates the last logged in user.
        /// </summary>
        /// <param name="email">Email.</param>
		private void updateLastLoggedIn(string email)
		{

            var lastLoggedIn = new LastLoggedIn(email, DateTime.Now);
            lock (Session.Connection) 
            {
                Session.Connection.Insert(lastLoggedIn);
            }
		}

        /// <summary>
        /// Gets the last logged in user
        /// </summary>
        /// <returns>The last logged in.</returns>
        private string getLastLoggedIn()
        {

            string e = "";

            lock (Session.Connection) 
            {
                var last = Session.Connection.Table<LastLoggedIn>().OrderByDescending(l => l.date).FirstOrDefault();

                if (last == null) 
                {
                    return null;
                }

                e = last.email;
            }


            if (!(String.IsNullOrWhiteSpace(e))) 
            {
                return e;
            }
            else 
            {
                return null;
            }
        }
    }
}

