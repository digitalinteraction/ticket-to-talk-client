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
		public static readonly Color color_grey_transparent = new Color(218, 226, 223, 0.8);
		public static readonly Color color_white_transparent = new Color(255, 255, 250, 0.5);

        public static readonly string Font_IOS_BoldHeader = "Nunito-SemiBold";
        public static readonly string Font_IOS_Header = "Nunito-Regular";
        public static readonly string Font_IOS_Body = "OpenSans-Regular";

		public static readonly string Font_Android_BoldHeader = "Nunito-SemiBold.ttf#Nunito-SemiBold";
		public static readonly string Font_Android_Header = "Nunito-Regular.ttf#Nunito-SemiBold";
		public static readonly string Font_Android_Body = "OpenSans-Regular.ttf#OpenSans-Regular";

        public static readonly int TextSize_H1 = 42;
        public static readonly int TextSize_Large = 22;
        public static readonly int TextSize_Medium = 18;
        public static readonly int TextSize_Small = 16;

        public static readonly int Padding = 20;

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

