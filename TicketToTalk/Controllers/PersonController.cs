using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Person controller.
	/// </summary>
	public class PersonController
	{

		PersonDB personDB = new PersonDB();
		NetworkController networkController = new NetworkController();

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
		public Person getPerson(int id)
		{
			personDB.open();
			var person = personDB.GetPerson(id);
			personDB.close();
			return person;
		}

		/// <summary>
		/// Gets the people.
		/// </summary>
		/// <returns>The people.</returns>
		public List<Person> getPeople()
		{
			var personUserDB = new PersonUserDB();
			var relations = personUserDB.getRelationByUserID(Session.activeUser.id);

			var list = new List<Person>();
			personDB.open();

			foreach (PersonUser pu in relations)
			{
				if (pu.user_id == Session.activeUser.id)
				{
					list.Add(personDB.GetPerson(pu.person_id));
				}
			}

			personDB.close();
			return list;
		}

		/// <summary>
		/// Deletes the person locally.
		/// </summary>
		/// <returns>The person locally.</returns>
		/// <param name="id">Identifier.</param>
		public void deletePersonLocally(int id)
		{
			personDB.open();
			personDB.DeletePerson(id);
			personDB.close();
		}

		/// <summary>
		/// Adds the person locally.
		/// </summary>
		/// <returns>The person locally.</returns>
		/// <param name="p">P.</param>
		public void addPersonLocally(Person p)
		{
			Debug.WriteLine("PersonController: adding person " + p);

			if (getPerson(p.id) == null)
			{
				personDB.open();
				personDB.AddPerson(p);
				personDB.close();
			}

			Debug.WriteLine("PersonController: person added");

		}

		/// <summary>
		/// Updates the person locally.
		/// </summary>
		/// <returns>The person locally.</returns>
		/// <param name="p">P.</param>
		public void updatePersonLocally(Person p)
		{
			deletePersonLocally(p.id);
			addPersonLocally(p);
		}

		/// <summary>
		/// Gets the person profile picture.
		/// </summary>
		/// <returns>The person profile picture.</returns>
		/// <param name="person">Person.</param>
		public async Task<ImageSource> getPersonProfilePicture(Person person)
		{
			ImageSource imageSource;
			if (person.pathToPhoto.Equals("default_profile.png"))
			{
				Debug.WriteLine("PersonController: Getting default image.");
				imageSource = ImageSource.FromFile(person.pathToPhoto);
			}
			else if (person.pathToPhoto.StartsWith("storage", StringComparison.Ordinal))
			{
				Debug.WriteLine("PersonController: Getting image from server.");
				var image = await downloadPersonProfilePicture(person);
				//imageSource = ImageSource.FromStream(() => new MemoryStream(downloadPersonProfilePicture(person)));
				imageSource = ImageSource.FromStream(() => new MemoryStream(image));
			}
			else
			{
				Debug.WriteLine("PersonController: Getting image from storage");
				var rawBytes = MediaController.readBytesFromFile(person.pathToPhoto);
				Debug.WriteLine("PersonController: fileSize " + rawBytes.Length);
				imageSource = ImageSource.FromStream(() => new MemoryStream(rawBytes));
			}
			return imageSource;
		}

		/// <summary>
		/// Downloads the person profile picture.
		/// </summary>
		/// <returns>The person profile picture.</returns>
		/// <param name="person">Person.</param>
		public async Task<byte[]> downloadPersonProfilePicture(Person person)
		{
			var download_finished = false;

			MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			{
				Debug.WriteLine("Image Downloaded");
				download_finished = finished;
			});

			var fileName = "p_" + person.id + ".jpg";

			//var task = Task.Run(() => networkController.downloadFile(person.pathToPhoto, fileName)).Result;
			var downloaded = await networkController.downloadFile(person.pathToPhoto, fileName);

			if (downloaded)
			{
				person.pathToPhoto = fileName;

				//while (!download_finished)
				//{
				//}
				while (PersonDB.locked) { }
				PersonDB.locked = true;
				updatePersonLocally(person);
				PersonDB.locked = false;
			}

			MessagingCenter.Unsubscribe<NetworkController, bool>(this, "download_image");

			return MediaController.readBytesFromFile(person.pathToPhoto);
		}

		/// <summary>
		/// Downloads the person profile picture for invite.
		/// </summary>
		/// <returns>The person profile picture for invite.</returns>
		/// <param name="person">Person.</param>
		public string downloadPersonProfilePictureForInvite(Person person)
		{
			var download_finished = false;

			MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			{
				Debug.WriteLine("Image Downloaded");
				download_finished = finished;
			});

			var fileName = "p_" + person.id + ".jpg";
			var task = Task.Run(() => networkController.downloadFile(person.pathToPhoto, fileName)).Result;

			person.pathToPhoto = fileName;

			while (!download_finished)
			{
			}

			MessagingCenter.Unsubscribe<NetworkController, bool>(this, "download_image");

			return fileName;
		}

		/// <summary>
		/// Gets the people from server.
		/// </summary>
		/// <returns>The people from server.</returns>
		public async Task<ObservableCollection<Person>> getPeopleFromServer()
		{
			// Setting parameters for request
			Console.WriteLine("Setting parameters for request");
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			// Sending request
			Console.WriteLine("Sending request");
			var jobject = await networkController.sendGetRequest("user/getpeople", parameters);
			Console.WriteLine(jobject);

			// Parsing JSON to People array
			Console.WriteLine("Parsing JSON to People array");
			var jpeople = jobject.GetValue("people");
			var peopleRaw = jpeople.ToObject<Person[]>();
			Array.Sort(peopleRaw);

			// Getting all people
			List<Person> savedPeople = new List<Person>();
			savedPeople = getPeople();

			// Checking if existing people have been updated externally.
			Console.WriteLine("Checking if existing people have been updated externally.");
			foreach (Person p in peopleRaw)
			{
				if (String.IsNullOrEmpty(p.pathToPhoto)) p.pathToPhoto = "path";
				if (String.IsNullOrEmpty(p.notes)) p.notes = "Add some notes about their condition";
				bool inSet = false;
				Debug.WriteLine(savedPeople.Count);
				foreach (Person s in savedPeople)
				{
					if (p.id == s.id)
					{
						inSet = true;

						if (!(p.updated_at.Equals(s.updated_at)))
						{
							Console.WriteLine("Updating person:" + s.id);
							p.pathToPhoto = s.pathToPhoto;
							p.imageHash = s.imageHash;
							updatePersonLocally(p);
						}
						if (p.imageHash != null)
						{
							if (s.imageHash == null)
							{
								downloadPersonProfilePicture(p);
							}
							else if (!(p.imageHash.Equals(s.imageHash)))
							{
								downloadPersonProfilePicture(p);
							}
						}
					}
				}

				PersonUserDB personUserDB = new PersonUserDB();
				if (!inSet)
				{
					Debug.WriteLine("Adding person and relation");
					var relation = new PersonUser(p.id, Session.activeUser.id, p.pivot.user_type, p.pivot.relation);
					personUserDB.AddPersonUser(relation);

					p.pivot.relation = null;
					p.pivot.user_type = null;
					p.pivot = null;
					Console.WriteLine(p);
					addPersonLocally(p);
				}
				personUserDB.close();
			}

			Debug.WriteLine("PersonController: Getting periods");

			var jtoken = jobject.GetValue("periods");
			Debug.WriteLine(jtoken.ToString());
			var periods = jtoken.ToObject<List<Period>>();

			var periodController = new PeriodController();
			var personPeriodDB = new PersonPeriodDB();
			personPeriodDB.open();
			foreach (Period p in periods)
			{
				Debug.WriteLine(p);
				var temp = periodController.getPeriod(p.id);
				if (temp == null)
				{
					periodController.addLocalPeriod(p);
				}

				var pp = new PersonPeriod((Int32.Parse(p.pivot.person_id)), Int32.Parse(p.pivot.period_id));
				var spp = personPeriodDB.getRelationByPersonAndPeriodID(pp.person_id, pp.period_id);

				if (spp == null)
				{
					personPeriodDB.AddPersonPeriodRelationship(pp);
				}
			}

			return new ObservableCollection<Person>(getPeople());
		}

		/// <summary>
		/// Deletes the person remotely.
		/// </summary>
		/// <returns><c>true</c>, if person was deleted remotely, <c>false</c> otherwise.</returns>
		/// <param name="person">Person.</param>
		public bool deletePersonRemotely(Person person)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = person.id.ToString();
			parameters["token"] = Session.Token.val;

			var jobject = networkController.sendDeleteRequest("people/destroy", parameters);

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
		public async Task<bool> addPersonRemotely(Person person, string relation, byte[] image)
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
				Debug.WriteLine("PersonController: imageHash = " + person.imageHash);
			}

			var net = new NetworkController();
			var jobject = await net.sendGenericPostRequest("people/store", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("person");
				var stored_person = jtoken.ToObject<Person>();


				stored_person.pathToPhoto = "default_profile.png";

				if (image != null)
				{
					var fileName = "p_" + stored_person.id + ".jpg";
					stored_person.pathToPhoto = fileName;
					MediaController.writeImageToFile(fileName, image);
				}

				addPersonLocally(stored_person);
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

				addStockPeriods(stored_person);

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
		public string getRelationship(int id)
		{
			var personUserDB = new PersonUserDB();
			var relation = personUserDB.getRelationByUserAndPersonID(Session.activeUser.id, id);
			personUserDB.close();
			return relation.relationship;
		}

		public PersonUser getRelation(int person_id)
		{
			var personUserDB = new PersonUserDB();
			var relation = personUserDB.getRelationByUserAndPersonID(Session.activeUser.id, person_id);
			personUserDB.close();

			return relation;
		}

		/// <summary>
		/// Adds the stock periods.
		/// </summary>
		/// <returns>The stock periods.</returns>
		/// <param name="p">P.</param>
		public void addStockPeriods(Person p)
		{
			PersonPeriodDB ppDB = new PersonPeriodDB();
			ppDB.open();
			for (int i = 1; i < 5; i++)
			{
				var pp = new PersonPeriod(p.id, i);
				ppDB.AddPersonPeriodRelationship(pp);
			}
			ppDB.close();
		}

		/// <summary>
		/// Gets the users.
		/// </summary>
		/// <returns>The users.</returns>
		/// <param name="id">Identifier.</param>
		public async Task<List<User>> getUsers(int id)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;
			parameters["person_id"] = id.ToString();
			string url = "people/getusers";

			// Send request for all users associated with the person
			Console.WriteLine("Sending request for all users associated with the person.");
			var jobject = await networkController.sendGetRequest(url, parameters);
			Console.WriteLine(jobject);

			var jusers = jobject.GetValue("users");
			var users = jusers.ToObject<User[]>();
			foreach (User u in users)
			{
				Console.WriteLine(u);
			}
			return new List<User>(users);
		}

		/// <summary>
		/// Updates the person remotely.
		/// </summary>
		/// <returns>The person remotely.</returns>
		/// <param name="person">Person.</param>
		public async Task<Person> updatePersonRemotely(Person person, byte[] image)
		{
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["person_id"] = person.id.ToString();
			parameters["name"] = person.name;
			parameters["birthPlace"] = person.birthPlace;
			parameters["birthYear"] = person.birthYear;
			parameters["notes"] = person.notes;
			parameters["area"] = person.area;
			parameters["image"] = null;
			parameters["imageHash"] = null;
			parameters["token"] = Session.Token.val;

			if (image != null)
			{
				parameters["image"] = image;
				parameters["imageHash"] = image.HashArray();
			}

			var jobject = await networkController.sendGenericPostRequest("people/update", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("Person");
				var p = jtoken.ToObject<Person>();

				updatePersonLocally(p);

				var r = AllProfiles.people.IndexOf(person);
				AllProfiles.people[r].name = p.name;
				AllProfiles.people[r].birthYear = p.birthYear;
				AllProfiles.people[r].birthPlace = p.birthPlace;
				AllProfiles.people[r].area = p.area;
				AllProfiles.people[r].notes = p.notes;
				AllProfiles.people[r].displayString = getDisplayString(AllProfiles.people[r]);
				AllProfiles.people[r].imageSource = ImageSource.FromStream(() => new MemoryStream(image));

				PersonProfile.currentPerson.notes = p.notes;
				PersonProfile.currentPerson.displayString = getDisplayString(p);
				PersonProfile.currentPerson.imageSource = ImageSource.FromStream(() => new MemoryStream(image));
				Session.activePerson = p;

				MediaController.writeImageToFile("p_" + p.id + ".jpg", image);

				return p;
			}

			return null;
		}

		/// <summary>
		/// Gets the display string.
		/// </summary>
		/// <returns>The display string.</returns>
		/// <param name="person">Person.</param>
		public string getDisplayString(Person person)
		{
			var displayString = String.Empty;

			if (!(String.IsNullOrEmpty(person.area)))
			{
				displayString = String.Format("Born in {0}, {1}\nSpent most of their life in {2}", person.birthPlace, person.birthYear, person.area);
			}
			else
			{
				displayString = String.Format("Born in {0}, {1}", person.birthPlace, person.birthYear);
			}

			return displayString;
		}
	}
}

