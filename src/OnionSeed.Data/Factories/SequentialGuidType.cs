using System.Diagnostics.CodeAnalysis;

namespace OnionSeed.Data.Factories
{
	/// <summary>
	/// Identifies the different ways a sequential GUID can be structured.
	/// </summary>
	/// <remarks>Different databases store GUID values in different ways internally.
	/// If this is not taken into consideration, GUID values can be much slower to persist than simple integer values.
	/// But by selecting a format that matches a database's storage particulars, we can largely negate that overhead.</remarks>
	public enum SequentialGuidType
	{
		/// <summary>
		/// Indicates that the GUID values will be stored as strings in the database,
		/// with the sequential data at the beginning of the string value of the GUID.
		/// <p>This works well for:
		/// <list type="bullet">
		///   <item>
		///     <description>MySQL (using <c>char(36)</c>)</description>
		///   </item>
		///   <item>
		///     <description>PostgreSQL (using <c>uuid</c>)</description>
		///   </item>
		///   <item>
		///     <description>SQLite (when stored as 36 characters of text)</description>
		///   </item>
		/// </list></p></summary>
		[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Approved as part of this enum.")]
		String = 0,

		/// <summary>
		/// Indicates that the GUID values will be stored as raw binary data.
		/// <p>This works well for:
		/// <list type="bullet">
		///   <item>
		///     <description>Oracle (using <c>raw(16)</c>)</description>
		///   </item>
		///   <item>
		///     <description>SQLite (when stored as 16 bytes of binary data)</description>
		///   </item>
		/// </list></p></summary>
		Binary = 1,

		/// <summary>
		/// Indicates that the GUID values will be stored as raw binary data,
		/// with the least-significant bytes containing the sequential data.
		/// <p>This works well for:
		/// <list type="bullet">
		///   <item>
		///     <description>SQL Server (using <c>uniqueidentifier</c>)</description>
		///   </item>
		/// </list></p></summary>
		BinaryEnd = 2
	}
}
