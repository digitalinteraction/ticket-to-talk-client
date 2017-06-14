using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace TicketToTalk
{

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        private static Random rng = new Random();

        /// <summary>
        /// Shuffle the specified list.
        /// Source: http://stackoverflow.com/questions/273313/randomize-a-listt
        /// </summary>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Hashs the array.
        /// </summary>
        /// <returns>The array.</returns>
        /// <param name="arr">Arr.</param>
        public static string HashArray(this byte[] arr)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(arr);
            var hashString = ByteToHex(hash);
            return hashString;
        }

        /// <summary>
        /// Hashs the string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="str">String.</param>
        public static string HashString(this string str)
        {
            var arr = Encoding.UTF8.GetBytes(str);
            return arr.HashArray();
        }

        /// <summary>
        /// Bytes to hex.
        /// Source: http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
        /// </summary>
        /// <returns>The to hex.</returns>
        /// <param name="ba">Ba.</param>
        private static string ByteToHex(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:X2}", b);
            }
            return hex.ToString();
        }

        /// <summary>
        /// Sets header on navigation page.
        /// </summary>
        /// <param name="nav">Nav.</param>
        public static void SetNavHeaders(this NavigationPage nav)
        {
            nav.BarTextColor = ProjectResource.color_white;
            nav.BarBackgroundColor = ProjectResource.color_blue;
        }

        /// <summary>
        /// Sets the button style.
        /// </summary>
        /// <param name="button">Button.</param>
        /// <param name="color">Color.</param>
        public static void SetButtonStyle(this Button button, Color color)
        {
            button.TextColor = ProjectResource.color_white;
            button.BackgroundColor = color;
            button.WidthRequest = (Session.ScreenWidth * 0.5);
            button.HorizontalOptions = LayoutOptions.CenterAndExpand;
        }

        /// <summary>
        /// Sets the label style.
        /// </summary>
        /// <param name="label">Label.</param>
        public static void SetLabelStyleInversreCenter(this Label label)
        {
            label.TextColor = ProjectResource.color_white;
            label.HorizontalTextAlignment = TextAlignment.Center;
            label.HorizontalOptions = LayoutOptions.Center;
            label.WidthRequest = Session.ScreenWidth * 0.8;
        }

        /// <summary>
        /// Sets the label style.
        /// </summary>
        /// <param name="label">Label.</param>
        public static void SetLabelStyleCenter(this Label label)
        {
            label.TextColor = ProjectResource.color_dark;
            label.HorizontalTextAlignment = TextAlignment.Center;
            label.HorizontalOptions = LayoutOptions.Center;
            label.WidthRequest = Session.ScreenWidth * 0.8;
        }

        /// <summary>
        /// Sets the label as header.
        /// </summary>
        /// <param name="label">Label.</param>
        public static void SetHeaderStyle(this Label label)
        {
            label.TextColor = ProjectResource.color_dark;
            label.FontAttributes = FontAttributes.Bold;
            label.HorizontalOptions = LayoutOptions.CenterAndExpand;
            label.VerticalOptions = LayoutOptions.CenterAndExpand;
            label.HorizontalTextAlignment = TextAlignment.Center;
            label.FontSize = ProjectResource.TextSize_Large;

#if __IOS__
            label.FontFamily = Device.OnPlatform(ProjectResource.Font_IOS_BoldHeader, null, null);
#else
            label.FontFamily = Device.OnPlatform(null, ProjectResource.Font_Android_BoldHeader, null);
#endif
		}

        /// <summary>
        /// Sets the label as sub header.
        /// </summary>
        /// <param name="label">Label.</param>
		public static void SetSubHeaderStyle(this Label label)
		{
			label.TextColor = ProjectResource.color_dark;
			label.FontAttributes = FontAttributes.Bold;
			label.HorizontalOptions = LayoutOptions.StartAndExpand;
			label.VerticalOptions = LayoutOptions.CenterAndExpand;
            label.FontSize = ProjectResource.TextSize_Medium;

#if __IOS__
            label.FontFamily = Device.OnPlatform(ProjectResource.Font_IOS_Header, null, null);
#else
            label.FontFamily = Device.OnPlatform(null, ProjectResource.Font_Android_Header, null);
#endif
		}

		public static void SetBodyStyle(this Label label)
		{
			label.TextColor = ProjectResource.color_dark;
			//label.FontAttributes = FontAttributes.Bold;
			label.HorizontalOptions = LayoutOptions.StartAndExpand;
			label.VerticalOptions = LayoutOptions.CenterAndExpand;
			label.FontSize = ProjectResource.TextSize_Small;

#if __IOS__
			label.FontFamily = Device.OnPlatform(ProjectResource.Font_IOS_Header, null, null);
#else
            label.FontFamily = Device.OnPlatform(null, ProjectResource.Font_Android_Header, null);
#endif
		}

        /// <summary>
        /// Sets the entry font.
        /// </summary>
        /// <param name="entry">Entry.</param>
        public static void SetStyle(this Entry entry) 
        {
#if __IOS__
            entry.FontFamily = Device.OnPlatform(ProjectResource.Font_IOS_Body, null, null);
#else
			entry.FontFamily = Device.OnPlatform(null, ProjectResource.Font_Android_Body, null);
#endif
		}

        /// <summary>
        /// Sets the button font.
        /// </summary>
        /// <param name="button">Button.</param>
        public static void SetStyle(this Button button) 
        {
            button.FontSize = ProjectResource.TextSize_Medium;
            button.Margin = new Thickness(0, 0, 0, 10);
            button.BorderRadius = 5;
#if __IOS__
            button.FontFamily = Device.OnPlatform(ProjectResource.Font_IOS_BoldHeader, null, null);
#else
            button.FontFamily = Device.OnPlatform(null, ProjectResource.Font_Android_BoldHeader, null);
#endif
		}

        /// <summary>
        /// Sets the style.
        /// </summary>
        /// <param name="editor">Editor.</param>
		public static void SetStyle(this Editor editor)
		{
#if __IOS__
            editor.FontFamily = Device.OnPlatform(ProjectResource.Font_IOS_Body, null, null);
#else
            editor.FontFamily = Device.OnPlatform(null, ProjectResource.Font_Android_Body, null);
#endif
		}

		/// <summary>
		/// Gets data from a API JSON response.
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="res">Res.</param>
		public static JToken GetData(this JObject res) 
		{

			var jstatus = res.GetValue("status");
			var code = jstatus["code"].ToObject<int>();

			var errors = res.GetValue("errors").ToObject<bool>();
			var data = res.GetValue("data");

			if (errors) 
			{
				switch (code) 
				{
					case 401:
						throw new APIUnauthorisedException("Unauthorised Access");
					case 403:
						throw new APIUnauthorisedForResourceException("Unauthorised for resource");
					case 404:
						throw new APIResourceNotFoundException("Resource not found");
					case 500:
						throw new APIErrorException("API Error");
				}
			}

			return data;
		}
	}
}

