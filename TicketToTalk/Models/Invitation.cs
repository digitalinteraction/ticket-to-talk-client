using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Invitation.
	/// </summary>
	public class Invitation : INotifyPropertyChanged
	{

		private ImageSource _imageSource;

		public Person person { get; set; }
		public string name { get; set; }
		public string group { get; set; }
		public string pathToPhoto { get; set; }
		public string person_name { get; set; }
		public ImageSource imageSource
		{
			get
			{
				return _imageSource;
			}
			set
			{
				_imageSource = value;
				NotifyPropertyChanged();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Invitation"/> class.
		/// </summary>
		public Invitation()
		{
		}

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}

