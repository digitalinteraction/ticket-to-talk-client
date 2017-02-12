// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 10/02/2017
//
// About.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
	public class About : ContentPage
	{
		public About()
		{
			this.Padding = 20;
			this.Title = "About";

			var p1 = new Label
			{
				Text = "The Ticket to Talk application is the result of a collaboration between Newcastle University and Youth Focus North East. It’s development and design was funded by the research projects: ESRC-IAA ‘Coproduction of dementia-care communications technology’ (DemYouth) project, the EPSRC Digital Economy Research Centre and the EPSRC Centre for Doctoral Training in Digital Civics.",
				TextColor = ProjectResource.color_dark,
				FontSize = 16
			};

			var p2 = new Label
			{
				Text = "It is a follow-on from the DemTalk project: www.demtalk.org.uk",
				TextColor = ProjectResource.color_dark,
				FontSize = 16
			};

			var p3 = new Label
			{
				Text = "For further information or to give comments and feedback, please contact Daniel Welsh: d.welsh@newcastle.ac.uk",
				TextColor = ProjectResource.color_dark,
				FontSize = 16
			};

			var nuLogo = new Image
			{
				Aspect = Aspect.AspectFit,
				MinimumHeightRequest = 200,
				WidthRequest = 200
			};
			nuLogo.Source = ImageSource.FromFile("NewcastleUni.jpg");

			var youthFocus = new Image
			{
				Aspect = Aspect.AspectFit,
				MinimumHeightRequest = 200,
				WidthRequest = 200
			};
			youthFocus.Source = ImageSource.FromFile("YFNE.jpg");

			var esrc = new Image
			{
				Aspect = Aspect.AspectFit,
				MinimumHeightRequest = 200,
				WidthRequest = 200
			};
			esrc.Source = ImageSource.FromFile("ESRC.jpg");

			var digitalEconomy = new Image
			{
				Aspect = Aspect.AspectFit,
				MinimumHeightRequest = 200,
				WidthRequest = 200
			};
			digitalEconomy.Source = ImageSource.FromFile("DigitalEconomy.jpg");

			var epsrc = new Image
			{
				Aspect = Aspect.AspectFit,
				MinimumHeightRequest = 200,
				WidthRequest = 200
			};
			epsrc.Source = ImageSource.FromFile("EPSRC.jpg");

			var nuNISR = new Image
			{
				Aspect = Aspect.AspectFit,
				MinimumHeightRequest = 200,
				WidthRequest = 200
			};
			nuNISR.Source = ImageSource.FromFile("NewcastleUniNISR.jpg");

			var stack = new StackLayout 
			{
				Children = {
					p1,
					p2,
					p3,
					nuLogo,
					youthFocus,
					esrc,
					digitalEconomy,
					epsrc,
					nuNISR
				}
			};
			Content = new ScrollView
			{
				Content = stack
			};
		}
	}
}

