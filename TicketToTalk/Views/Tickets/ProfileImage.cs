
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
	public class ProfileImage : ContentPage
	{
		public ProfileImage()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}


