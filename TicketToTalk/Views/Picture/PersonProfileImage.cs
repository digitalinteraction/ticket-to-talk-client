// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 17/08/2016
//
// ProfileImage.cs
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
				Source = Task.Run(() => personController.GetPersonProfilePicture(person)).Result
			};

			Content = profilePic;
		}
	}
}


