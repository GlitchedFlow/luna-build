using Luna.Core;
using Luna.Core.Target;

namespace Luna.Targets.VisualStudio.Projects
{
	public enum VisualStudioProjectType
	{
		None = 0,
		CSharp,
		Cpp,
		Folder
	}

	/// <summary>
	/// Abstract base class for a visual studio project.
	/// </summary>
	/// <param name="typeGuid">Guid for the type of project.</param>
	/// <param name="name">Name of the project.</param>
	/// <param name="relativePath">Relative project path from the solution.</param>
	/// <param name="guid">Project guid.</param>
	/// <param name="isFolder">Declares this project as a folder.</param>
	public abstract class BaseProject(Guid typeGuid, string name, string relativePath, Guid guid) : IProject
	{
		/// <summary>
		/// Gets the guid of the type of this project.
		/// </summary>
		public Guid TypeGuid => typeGuid;

		/// <summary>
		/// Gets the name of the project.
		/// </summary>
		public string Name => name;

		/// <summary>
		/// Gets the relative project path from the solution.
		/// </summary>
		public string RelativePath => relativePath;

		/// <summary>
		/// Gets the guid of this project.
		/// </summary>
		public Guid Guid => guid;

		/// <summary>
		/// Gets the file extension of this project.
		/// </summary>
		public abstract string Extension { get; }

		/// <summary>
		/// Write the project to file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		public abstract bool WriteFile();

		public static Guid ProjectToGuid(VisualStudioProjectType type)
		{
			switch (type)
			{
				case VisualStudioProjectType.None:
					return Guid.Empty;

				case VisualStudioProjectType.CSharp:
					return "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}".AsGuid();

				case VisualStudioProjectType.Cpp:
					return "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}".AsGuid();

				case VisualStudioProjectType.Folder:
					return "{2150E333-8FDC-42A3-9474-1A3956D46DE8}".AsGuid();

				default:
					return Guid.Empty;
			}
		}
	}
}