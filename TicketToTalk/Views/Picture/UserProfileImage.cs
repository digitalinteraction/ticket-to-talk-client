// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 17/08/2016
//
// UserProfileImage.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// User profile image.
	/// </summary>
	public class UserProfileImage : ContentView
	{
		bool download_finished;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.UserProfileImage"/> class.
		/// </summary>
		public UserProfileImage(double size, string alignment, Color borderColour)
		{
			//MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			//{
			//	Debug.WriteLine("Image Downloaded");
			//	download_finished = finished;
			//});

			CircleImage profilePic = new CircleImage
			{
				BorderColor = ProjectResource.color_red,
				BorderThickness = 2,
				Aspect = Aspect.AspectFill,
				HeightRequest = (Session.ScreenWidth * 0.8),
				WidthRequest = (Session.ScreenWidth * 0.8),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(20),
			};
			profilePic.SetBinding(Image.SourceProperty, "imageSource");
			profilePic.BindingContext = Session.activeUser;

			if (size > 0.1)
			{
				profilePic.HeightRequest = size;
				profilePic.WidthRequest = size;
			}
			if (!(String.IsNullOrEmpty(alignment))) 
			{
				switch (alignment) 
				{
					case ("left"):
						profilePic.HorizontalOptions = LayoutOptions.Start;
						break;
					case ("centre"):
						profilePic.HorizontalOptions = LayoutOptions.Center;
						break;
					case ("right"):
						profilePic.HorizontalOptions = LayoutOptions.End;
						break;
				}
			}
			profilePic.BorderColor = borderColour;

			//if (user.pathToPhoto.StartsWith("storage", StringComparison.Ordinal))
			//{
			//	NetworkController net = new NetworkController();
			//	var fileName = "u_" + user.id + ".jpg";
			//	var task = Task.Run(() => net.downloadFile(user.pathToPhoto, fileName)).Result;
			//	user.pathToPhoto = fileName;

			//	while (!download_finished)
			//	{
			//	}

			//	var rawBytes = MediaController.readBytesFromFile(user.pathToPhoto);
			//	profilePic.Source = ImageSource.FromStream(() => new MemoryStream(rawBytes));

			//	var userController = new UserController();
			//	userController.updateUserLocally(user);
			//	Session.activeUser = user;

			//	//MessagingCenter.Unsubscribe<NetworkController, bool>(this, "download_image");
			//}
			//else
			//{
			//	var rawBytes = MediaController.readBytesFromFile(user.pathToPhoto);
			//	profilePic.Source = ImageSource.FromStream(() => new MemoryStream(rawBytes));
			//}

			//MessagingCenter.Unsubscribe<NetworkController, bool>(this, "download_image");

			Content = profilePic;
		}
	}
}


