using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class PersonController
	{

		PersonDB personDB = new PersonDB();

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
			personDB.open();
			personDB.AddPerson(p);
			personDB.close();
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
		/// Downloads the person profile picture.
		/// </summary>
		/// <returns>The person profile picture.</returns>
		/// <param name="person">Person.</param>
		public byte[] downloadPersonProfilePicture(Person person) 
		{
			var download_finished = false;

			MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			{
				Debug.WriteLine("Image Downloaded");
				download_finished = finished;
			});

			NetworkController net = new NetworkController();
			var fileName = "p_" + person.id + ".jpg";
			var task = Task.Run(() => net.downloadImage(person.pathToPhoto, fileName)).Result;

			person.pathToPhoto = fileName;

			while (!download_finished)
			{
			}
			updatePersonLocally(person);

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

			NetworkController net = new NetworkController();
			var fileName = "p_" + person.id + ".jpg";
			var task = Task.Run(() => net.downloadImage(person.pathToPhoto, fileName)).Result;

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
			NetworkController net = new NetworkController();
			var jobject = await net.sendGetRequest("user/getpeople", parameters);
			Console.WriteLine(jobject);

			// Parsing JSON to People array
			Console.WriteLine("Parsing JSON to People array");
			var jpeople = jobject.GetValue("people");
			var peopleRaw = jpeople.ToObject<Person[]>();

			// Getting all people
			Console.WriteLine("Getting all people");
			//PersonDB personDB = new PersonDB();
			//var personController = new PersonController();
			List<Person> savedPeople = new List<Person>();
			//savedPeople = personDB.GetPersons();
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
							//personDB.DeletePerson(s.id);
							//personDB.AddPerson(s);
							updatePersonLocally(s);
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

				var pp = new PersonPeriod(Int32.Parse(p.pivot.person_id), Int32.Parse(p.pivot.period_id));
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

			var networkController = new NetworkController();
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
			NetworkController net = new NetworkController();
			var jobject = await net.sendGetRequest(url, parameters);
			Console.WriteLine(jobject);

			var jusers = jobject.GetValue("users");
			var users = jusers.ToObject<User[]>();
			foreach (User u in users)
			{
				Console.WriteLine(u);
			}
			return new List<User>(users);
		}
	}
}

