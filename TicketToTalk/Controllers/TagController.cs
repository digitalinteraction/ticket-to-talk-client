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
		private TagDB tagDB = new TagDB();

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
			tagDB.Open();
			tagDB.AddTag(tag);
			tagDB.Close();
		}

		/// <summary>
		/// Deletes the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteTagLocally(int id) 
		{
			tagDB.Open();
			tagDB.DeleteTag(id);
			tagDB.Close();
		}

		/// <summary>
		/// Updates the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="t">T.</param>
		public void UpdateTagLocally(Tag t) 
		{
			DeleteTagLocally(t.id);
			AddTagLocally(t);
		}

		/// <summary>
		/// Gets the tag.
		/// </summary>
		/// <returns>The tag.</returns>
		/// <param name="id">Identifier.</param>
		public Tag GetTag(int id) 
		{
			tagDB.Open();
			var tag = tagDB.GetTag(id);
			tagDB.Close();
			return tag;
		}

		public List<Tag> GetTags() 
		{
			tagDB.Open();
			var tags = tagDB.GetTags();
			tagDB.Close();

			var list = new List<Tag>();
			foreach (Tag t in tags) 
			{
				list.Add(t);
			}
			return list;
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

