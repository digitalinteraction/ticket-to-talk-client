// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 17/08/2016
//
// ProfileImage.cs
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Profile image.
	/// </summary>
	public class PersonProfileImage : ContentView
	{
		public CircleImage profilePic;
		PersonController personController = new PersonController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ProfileImage"/> class.
		/// </summary>
		public PersonProfileImage(Person person)
		{
			//MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			//{
			//	Debug.WriteLine("Image Downloaded");
			//	download_finished = finished;
			//});

			profilePic = new CircleImage
			{
				BorderColor = ProjectResource.color_blue,
				BorderThickness = 2,
				HeightRequest = (Session.ScreenWidth * 0.8),
				WidthRequest = (Session.ScreenWidth * 0.8),
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(20),
				Source = personController.getPersonProfilePicture(person)
			};
			//profilePic.SetBinding(Image.SourceProperty, "imageSource");

			Content = profilePic;
		}
	}
}


