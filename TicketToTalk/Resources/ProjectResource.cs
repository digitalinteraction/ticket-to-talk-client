using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class ProjectResource
	{
		public static readonly Color color_white = Color.FromHex("FFFFFA");
		public static readonly Color color_red = Color.FromHex("FF5E5B");
		public static readonly Color color_blue = Color.FromHex("00CECB");
		public static readonly Color color_yellow = Color.FromHex("FFED66");
		public static readonly Color color_dark = Color.FromHex("233D4D");
		public static readonly Color color_grey = Color.FromHex("DAE2DF");

		public static readonly Color color_blue_transparent = new Color(0, 206, 203, 0.2);
		public static readonly Color color_grey_transparent = new Color(218, 226, 223, 0.2);

		public static readonly string[] relations =
		{
			"Father",
			"Mother",
			"Grandfather",
			"Grandmother",
			"Uncle",
			"Aunt",
			"Friend",
		};

		public static readonly string[] groups =
		{
			"All",
			"Family",
			"Friends",
			"Other (Professionals)"
		};

		public ProjectResource()
		{
		}
	}
}

