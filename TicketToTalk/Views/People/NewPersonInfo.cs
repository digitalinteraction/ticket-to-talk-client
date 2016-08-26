// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 25/08/2016
//
// NewPersonInfo.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// New person info.
	/// </summary>
	public class NewPersonInfo : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewPersonInfo"/> class.
		/// </summary>
		public NewPersonInfo()
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


