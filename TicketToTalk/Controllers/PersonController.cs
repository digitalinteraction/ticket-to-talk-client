using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Person controller.
	/// </summary>
	public class PersonController
	{

		private PersonDB personDB = new PersonDB();
		private NetworkController networkController = new NetworkController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PersonController"/> class.
		/// </summary>
		public PersonController()
		{
		}

		/// <summary>
		/// Gets the person.
		/// </summary>
		/// <returns>The person.</returns>
		/// <param name="id">Identifier.</param>
		public Person GetPerson(int id)
		{
			personDB.Open();
			var person = personDB.GetPerson(id);
			personDB.Close();
			return person;
		}

		/// <summary>
		/// Gets the people.
		/// </summary>
		/// <returns>The people.</returns>
		public List<Person> GetPeople()
		{
			var personUserDB = new PersonUserDB();
			var relations = personUserDB.GetRelationByUserID(Session.activeUser.id);

			var list = new List<Person>();
			personDB.Open();

			foreach (PersonUser pu in relations)
			{
				if (pu.user_id == Session.activeUser.id)
				{
					list.Add(personDB.GetPerson(pu.person_id));
				}
			}

			personDB.Close();
			return list;
		}

		/// <summary>
		/// Deletes the person locally.
		/// </summary>
		/// <returns>The person locally.</returns>
		/// <param name="id">Identifier.</param>
		public void DeletePersonLocally(int id)
		{
			personDB.Open();
			personDB.DeletePerson(id);
			personDB.Close();
		}

		/// <summary>
		/// Adds the person locally.
		/// </summary>
		/// <returns>The person locally.</returns>
		/// <param name="p">P.</param>
		public void AddPersonLocally(Person p)
		{
			Console.WriteLine("Adding person");

			if (GetPerson(p.id) == null)
			{
				personDB.Open();
				personDB.AddPerson(p);
				personDB.Close();
			}

		}

		/// <summary>
		/// Updates the person locally.
		/// </summary>
		/// <returns>The person locally.</returns>
		/// <param name="p">P.</param>
		public void UpdatePersonLocally(Person p)
		{
			DeletePersonLocally(p.id);
			AddPersonLocally(p);
		}

		/// <summary>
		/// Updates the relationship locally.
		/// </summary>
		/// <param name="pu">Pu.</param>
		public void UpdateRelationshipLocally(PersonUser pu)
		{
			var puDB = new PersonUserDB();

			puDB.DeleteRelation(pu.id);
			puDB.AddPersonUser(pu);

			puDB.Close();
		}

		/// <summary>
		/// Gets the person profile picture.
		/// </summary>
		/// <returns>The person profile picture.</returns>
		/// <param name="person">Person.</param>
		public async Task<ImageSource> GetPersonProfilePicture(Person person)
		{
			ImageSource imageSource;
			if (person.pathToPhoto.Equals("default_profile.png"))
			{
				imageSource = ImageSource.FromFile(person.pathToPhoto);
			}
			else if (person.pathToPhoto.StartsWith("ticket_to_talk", StringComparison.Ordinal))
			{
				var image = await DownloadPersonProfilePicture(person);
				imageSource = ImageSource.FromStream(() => new MemoryStream(image));
			}
			else
			{
				var rawBytes = MediaController.ReadBytesFromFile(person.pathToPhoto);
				imageSource = ImageSource.FromStream(() => new MemoryStream(rawBytes));
			}
			return imageSource;
		}

		/// <summary>
		/// Downloads the person profile picture.
		/// </summary>
		/// <returns>The person profile picture.</returns>
		/// <param name="person">Person.</param>
		public async Task<byte[]> DownloadPersonProfilePicture(Person person)
		{
			var fileName = "p_" + person.id + ".jpg";
			//var downloaded = await networkController.DownloadFile(person.pathToPhoto, fileName);
			var downloaded = await Task.Run(() => DownloadProfilePicture(person.id));

			if (downloaded)
			{
				person.pathToPhoto = fileName;

				while (PersonDB.locked) { }
				PersonDB.locked = true;
				UpdatePersonLocally(person);
				PersonDB.locked = false;
			}

			return MediaController.ReadBytesFromFile(person.pathToPhoto);
		}

		public async static Task<bool> DownloadProfilePicture(int id)
		{
			var client = new HttpClient();

			client.DefaultRequestHeaders.Host = "tickettotalk.openlab.ncl.ac.uk";
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			client.Timeout = new TimeSpan(0, 0, 100);

			var url = new Uri(Session.baseUrl + "people/picture?&token=" + Session.Token.val + "&api_key=" + Session.activeUser.api_key + "&person_id=" + id);

			Console.WriteLine("Beginning Download");
			Debug.WriteLine(url);

			Stream returned = null;

			try
			{
				returned = await client.GetStreamAsync(url);
			}
			catch (WebException ex)
			{
				Debug.WriteLine(ex.StackTrace);
				throw new NoNetworkException("No network available, check you are connected to the internet.");
			}
			catch (TaskCanceledException ex)
			{
				Debug.WriteLine(ex.StackTrace);
				throw new NoNetworkException("No network available, check you are connected to the internet.");
			}
			catch (HttpRequestException ex)
			{
				Debug.WriteLine(ex.StackTrace);
				throw new NoNetworkException("No network available, check you are connected to the internet.");
			}

			byte[] buffer = new byte[16 * 1024];
			byte[] imageBytes;
			using (MemoryStream ms = new MemoryStream())
			{
				int read = 0;
				while ((read = returned.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				imageBytes = ms.ToArray();
			}

			if (returned != null)
			{
				var fileName = "p_" + id + ".jpg";
				MediaController.WriteImageToFile(fileName, imageBytes);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the person profile picture for invite.
		/// </summary>
		/// <returns>The person profile picture for invite.</returns>
		/// <param name="person">Person.</param>
		public async Task<ImageSource> GetPersonProfilePictureForInvite(Person person)
		{
			if (person.pathToPhoto.Equals("default_profile.png"))
			{
				return ImageSource.FromFile(person.pathToPhoto);
			}
			else
			{
				return await DownloadPersonProfilePictureForInvite(person);
			}
		}

		/// <summary>
		/// Downloads the person profile picture for invite.
		/// </summary>
		/// <returns>The person profile picture for invite.</returns>
		/// <param name="person">Person.</param>
		public async Task<ImageSource> DownloadPersonProfilePictureForInvite(Person person)
		{
			var fileName = "p_" + person.id + ".jpg";
			await networkController.DownloadFile(person.pathToPhoto, fileName);

			person.pathToPhoto = fileName;

			return ImageSource.FromStream(() => new MemoryStream(MediaController.ReadBytesFromFile(fileName)));
		}

		/// <summary>
		/// Gets the people from server.
		/// </summary>
		/// <returns>The people from server.</returns>
		public async Task<ObservableCollection<Person>> GetPeopleFromServer()
		{
			// Setting parameters for request
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			// Sending request
			JObject jobject = null;

			try
			{
				jobject = await networkController.SendGetRequest("user/getpeople", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			// Parsing JSON to People array
			var data = jobject.GetData();

			var jpeople = data["people"];
			var peopleRaw = jpeople.ToObject<Person[]>();
			Array.Sort(peopleRaw);

			// Getting all people
			List<Person> savedPeople = new List<Person>();
			savedPeople = GetPeople();

			// Checking if existing people have been updated externally.
			foreach (Person p in peopleRaw)
			{
				if (string.IsNullOrEmpty(p.pathToPhoto)) p.pathToPhoto = "path";
				if (string.IsNullOrEmpty(p.notes)) p.notes = "Add some notes about their condition";
				bool inSet = false;
				foreach (Person s in savedPeople)
				{
					if (p.id == s.id)
					{
						inSet = true;

						if (!(p.updated_at.Equals(s.updated_at)))
						{
							p.pathToPhoto = s.pathToPhoto;
							p.imageHash = s.imageHash;
							UpdatePersonLocally(p);
						}
						if (p.imageHash != null)
						{
							if (s.imageHash == null)
							{
								await DownloadPersonProfilePicture(p);
							}
							else if (!(p.imageHash.Equals(s.imageHash)))
							{
								await DownloadPersonProfilePicture(p);
							}
						}
					}
				}

				var personUserDB = new PersonUserDB();
				if (!inSet)
				{
					var relation = new PersonUser(p.id, Session.activeUser.id, p.pivot.user_type, p.pivot.relation);
					personUserDB.AddPersonUser(relation);

					p.pivot.relation = null;
					p.pivot.user_type = null;
					p.pivot = null;
					AddPersonLocally(p);
				}
				personUserDB.Close();
			}

			var periods = data["periods"].ToObject<List<Period>>();

			var periodController = new PeriodController();
			var personPeriodDB = new PersonPeriodDB();
			personPeriodDB.Open();
			foreach (Period p in periods)
			{
				var temp = periodController.GetPeriod(p.id);
				if (temp == null)
				{
					periodController.AddLocalPeriod(p);
				}

				var pp = new PersonPeriod((int.Parse(p.pivot.person_id)), int.Parse(p.pivot.period_id));
				var spp = personPeriodDB.GetRelationByPersonAndPeriodID(pp.person_id, pp.period_id);

				if (spp == null)
				{
					personPeriodDB.AddPersonPeriodRelationship(pp);
				}
			}

			return new ObservableCollection<Person>(GetPeople());
		}

		/// <summary>
		/// Remove the person from remote and local databases.
		/// </summary>
		/// <returns>The person.</returns>
		/// <param name="person">Person.</param>
		public async Task<bool> DestroyPerson(Person person)
		{

			bool deleted = false;

			try
			{
				deleted = await DeletePersonRemotely(person);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			if (deleted)
			{
				DeletePersonLocally(person.id);

				var ticketController = new TicketController();
				var mediaController = new MediaController();
				var tickets = ticketController.GetTicketsByPerson(person.id);

				foreach (Ticket t in tickets)
				{
					ticketController.DeleteTicketLocally(t);
					mediaController.DeleteFile(t.pathToFile);
				}

				var relation = GetRelation(person.id);
				var personUserDB = new PersonUserDB();
				personUserDB.DeleteRelation(relation.id);

				AllProfiles.people.Remove(person);

				return true;
			}
			return false;
		}

		/// <summary>
		/// Deletes the person remotely.
		/// </summary>
		/// <returns><c>true</c>, if person was deleted remotely, <c>false</c> otherwise.</returns>
		/// <param name="person">Person.</param>
		public async Task<bool> DeletePersonRemotely(Person person)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = person.id.ToString();
			parameters["token"] = Session.Token.val;

			JObject jobject = null;

			try
			{
				jobject = await networkController.SendDeleteRequest("people/destroy", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			if (jobject == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Adds the person remotely.
		/// </summary>
		/// <returns>The person remotely.</returns>
		/// <param name="person">Person.</param>
		/// <param name="image">Image.</param>
		public async Task<bool> AddPersonRemotely(Person person, string relation, byte[] image)
		{
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["token"] = Session.Token.val;
			parameters["name"] = person.name;
			parameters["birthYear"] = person.birthYear;
			parameters["birthPlace"] = person.birthPlace;
			parameters["townCity"] = person.area;
			parameters["notes"] = person.notes;
			parameters["relation"] = relation;
			parameters["pathToPhoto"] = null;
			parameters["imageHash"] = null;
			parameters["image"] = image;

			if (image == null)
			{
				parameters["pathToPhoto"] = "default_profile.png";
			}
			else
			{
				person.imageHash = image.HashArray();
				parameters["imageHash"] = person.imageHash;
			}

			var net = new NetworkController();
			var jobject = await net.SendPostRequest("people/store", parameters);
			if (jobject != null)
			{

				var data = jobject.GetData();
				var jtoken = data["person"];
				var stored_person = jtoken.ToObject<Person>();


				stored_person.pathToPhoto = "default_profile.png";

				if (image != null)
				{
					var fileName = "p_" + stored_person.id + ".jpg";
					stored_person.pathToPhoto = fileName;
					MediaController.WriteImageToFile(fileName, image);
				}

				AddPersonLocally(stored_person);
				Session.activePerson = stored_person;

				var personUserDB = new PersonUserDB();
				var pu = new PersonUser
				{
					user_id = Session.activeUser.id,
					person_id = stored_person.id,
					relationship = relation,
					user_type = "Admin"
				};
				personUserDB.AddPersonUser(pu);

				AddStockPeriods(stored_person);

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the relationship.
		/// </summary>
		/// <returns>The relationship.</returns>
		/// <param name="id">Identifier.</param>
		public string GetRelationship(int id)
		{
			var personUserDB = new PersonUserDB();
			var relation = personUserDB.GetRelationByUserAndPersonID(Session.activeUser.id, id);
			personUserDB.Close();
			return relation.relationship;
		}

		/// <summary>
		/// Gets the relation between the user and the person.
		/// </summary>
		/// <returns>The relation.</returns>
		/// <param name="person_id">Person identifier.</param>
		public PersonUser GetRelation(int person_id)
		{
			var personUserDB = new PersonUserDB();
			var relation = personUserDB.GetRelationByUserAndPersonID(Session.activeUser.id, person_id);
			personUserDB.Close();

			return relation;
		}

		/// <summary>
		/// Adds the stock periods.
		/// </summary>
		/// <returns>The stock periods.</returns>
		/// <param name="p">P.</param>
		public void AddStockPeriods(Person p)
		{
			var ppDB = new PersonPeriodDB();
			ppDB.Open();
			for (int i = 1; i < 5; i++)
			{
				var pp = new PersonPeriod(p.id, i);
				ppDB.AddPersonPeriodRelationship(pp);
			}
			ppDB.Close();
		}

		/// <summary>
		/// Gets the users associated with a person.
		/// </summary>
		/// <returns>The users.</returns>
		/// <param name="id">Identifier.</param>
		public async Task<List<User>> GetUsers(int id)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;
			parameters["person_id"] = id.ToString();
			string url = "people/getusers";

			// Send request for all users associated with the person
			JObject jobject = null;

			try
			{
				jobject = await networkController.SendGetRequest(url, parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			var data = jobject.GetData();
			var jusers = data["users"];
			var users = jusers.ToObject<User[]>();

			return new List<User>(users);
		}

		/// <summary>
		/// Updates the person remotely.
		/// </summary>
		/// <returns>The person remotely.</returns>
		/// <param name="person">Person.</param>
		public async Task<Person> UpdatePersonRemotely(Person person, string relation, byte[] image)
		{

			// Build url parameters.
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["person_id"] = person.id.ToString();
			parameters["name"] = person.name;
			parameters["birthPlace"] = person.birthPlace;
			parameters["birthYear"] = person.birthYear;
			parameters["notes"] = person.notes;
			parameters["area"] = person.area;
			parameters["image"] = null;
			parameters["imageHash"] = null;
			parameters["relation"] = relation;
			parameters["token"] = Session.Token.val;

			if (image != null)
			{
				parameters["image"] = image;
				parameters["imageHash"] = image.HashArray();
			}

			// Make post request.
			var jobject = await networkController.SendPostRequest("people/update", parameters);
			if (jobject != null)
			{
				var data = jobject.GetData();

				// Get person token
				var jtoken = data["person"];
				var p = jtoken.ToObject<Person>();

				// Update local copy
				UpdatePersonLocally(p);

				// Create a new relationship between person and user.
				var new_relation = GetRelation(p.id);
				new_relation.relationship = relation;

				// Update local copy.
				UpdateRelationshipLocally(new_relation);

				// Parse into relation model.
				var pp = new PersonPivot();
				pp.relation = new_relation.relationship;

				// Update displayed instances of the person.
				var r = AllProfiles.people.IndexOf(person);
				AllProfiles.people[r].name = p.name;
				AllProfiles.people[r].birthYear = p.birthYear;
				AllProfiles.people[r].birthPlace = p.birthPlace;
				AllProfiles.people[r].area = p.area;
				AllProfiles.people[r].notes = p.notes;
				AllProfiles.people[r].displayString = GetDisplayString(AllProfiles.people[r]);

				PersonProfile.currentPerson.notes = p.notes;
				PersonProfile.currentPerson.displayString = GetDisplayString(p);
				PersonProfile.pivot.relation = pp.relation;

				Session.activePerson = p;

				if (image != null)
				{
					// Update image stored locally.
					AllProfiles.people[r].imageSource = ImageSource.FromStream(() => new MemoryStream(image));
					PersonProfile.currentPerson.imageSource = ImageSource.FromStream(() => new MemoryStream(image));
					MediaController.WriteImageToFile("p_" + p.id + ".jpg", image);
				}

				return p;
			}

			return null;
		}

		/// <summary>
		/// Gets the display string.
		/// </summary>
		/// <returns>The display string.</returns>
		/// <param name="person">Person.</param>
		public string GetDisplayString(Person person)
		{
			var displayString = string.Empty;

			if (!(string.IsNullOrEmpty(person.area)))
			{
				displayString = string.Format("Born in {0}, {1}\nSpent most of their life in {2}", person.birthPlace, person.birthYear, person.area);
			}
			else
			{
				displayString = string.Format("Born in {0}, {1}", person.birthPlace, person.birthYear);
			}

			return displayString;
		}
	}
}

