namespace OnionSeed.Data
{
	/// <summary>
	/// Defines an atomic transaction whose changes are to be committed as a collective whole.
	/// </summary>
	public interface IUnitOfWork
	{
		/// <summary>
		/// Commits any pending changes.
		/// </summary>
		void Commit();
	}
}
