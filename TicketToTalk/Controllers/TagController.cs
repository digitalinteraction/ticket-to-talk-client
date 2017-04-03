using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketToTalk
{
	/// <summary>
	/// Tag controller.
	/// </summary>
	public class TagController
	{
		public TagController()
		{
		}

		/// <summary>
		/// Adds the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="tag">Tag.</param>
		public void AddTagLocally(Tag tag) 
		{
			lock (Session.connection)
			{
				Session.connection.Insert(tag);
			}
		}

		/// <summary>
		/// Deletes the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteTagLocally(Tag tag) 
		{
			lock (Session.connection)
			{
				Session.connection.Delete(tag);
			}
		}

		/// <summary>
		/// Updates the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="t">T.</param>
		public void UpdateTagLocally(Tag t) 
		{
			lock (Session.connection)
			{
				Session.connection.Update(t);
			}
		}

		/// <summary>
		/// Gets the tag.
		/// </summary>
		/// <returns>The tag.</returns>
		/// <param name="id">Identifier.</param>
		public Tag GetTag(int id) 
		{
			Tag tag;

			lock (Session.connection)
			{
				tag = (from t in Session.connection.Table<Tag>() where t.id == id select t).FirstOrDefault();
			}

			return tag;
		}

		public List<Tag> GetTags() 
		{

			List<Tag> tags = new List<Tag>();

			lock (Session.connection)
			{
				var q = from t in Session.connection.Table<Tag>() select t;

				foreach (Tag tag in q) 
				{
					tags.Add(tag);
				}
			}

			return tags;
		}

		/// <summary>
		/// Adds the tag to server.
		/// </summary>
		/// <returns>if new tag</returns>
		/// <param name="tag">Tag.</param>
		public async Task<bool> AddTagToServer(Tag tag)
		{
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["text"] = tag.text;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			var jobject = await net.SendPostRequest("tags/store", parameters);

			var data = jobject.GetData();

			var jtoken = data["tag"];
			var returned_tag = jtoken.ToObject<Tag>();

			Console.WriteLine(tag);

			// Check tag already exists.
			var stored = GetTag(returned_tag.id);

			bool exists = false;
			if (stored != null)
			{
				exists = true;
			}
			else 
			{
				AddTagLocally(tag);
			}

			return exists;
		}
	}
}

