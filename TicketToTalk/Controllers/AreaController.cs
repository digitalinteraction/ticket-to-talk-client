namespace TicketToTalk
{
	/// <summary>
	/// A Controller for the Area Model.
	/// </summary>
	public class AreaController
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AreaController"/> class.
		/// </summary>
		public AreaController()
		{
		}

		/// <summary>
		/// Gets the area from an ID value.
		/// </summary>
		/// <returns>The area.</returns>
		/// <param name="id">Identifier.</param>
		public Area GetArea(int id)
		{
			Area area;

			lock(Session.connection) 
			{
				area = (from n in Session.connection.Table<Area>() where n.id == id select n).FirstOrDefault();
			}

			return area;
		}

		/// <summary>
		/// Adds the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="area">Area.</param>
		public void AddAreaLocally(Area area)
		{
			lock(Session.connection) 
			{
				Session.connection.Insert(area);
			}
		}

		/// <summary>
		/// Deletes the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteAreaLocally(Area area)
		{

			lock(Session.connection) 
			{
				Session.connection.Delete<Area>(area);
			}
		}

		/// <summary>
		/// Updates the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="area">Area.</param>
		public void UpdateAreaLocally(Area area)
		{
			lock(Session.connection) 
			{
				Session.connection.Update(area);
			}
		}
	}
}

