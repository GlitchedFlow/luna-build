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
	/// Simple record to describe a project tree in visual studio.
	/// </summary>
	/// <param name="Name">Name of the entry.</param>
	/// <param name="Value">Value of the entry.</param>
	/// <param name="Children">Children of the entry.</param>
	/// <param name="Attributes">Attrributes of the entry.</param>
	public record ProjectEntry(string Name, string Value, List<ProjectEntry> Children, Dictionary<string, string> Attributes);

	/// <summary>
	/// Abstract base class for a visual studio project.
	/// </summary>
	/// <param name="typeGuid">Guid for the type of project.</param>
	/// <param name="name">Name of the project.</param>
	/// <param name="relativePath">Relative project path from the solution.</param>
	/// <param name="guid">Project guid.</param>
	public abstract class BaseProject(Guid typeGuid, string name, string relativePath, Guid guid, Solution solution) : IProject
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
		/// Gets the solution this project belongs to.
		/// </summary>
		public Solution Solution => solution;

		/// <summary>
		/// Write the project to file.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		public abstract bool WriteFile();

		/// <summary>
		/// Returns Visual Studio specific guid for the given project type.
		/// </summary>
		/// <param name="type">Type of the project.</param>
		/// <returns>Visual Studio guid for given project type.</returns>
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

		/// <summary>
		/// Writes the Project into a string buffer.
		/// </summary>
		/// <param name="currentEntry">Current element of the project.</param>
		/// <param name="fileContent">String buffer</param>
		/// <param name="indentation">Indentation for the current element.</param>
		public static void WriteTreeToBuffer(ProjectEntry currentEntry, ref string fileContent, string indentation = "")
		{
			fileContent += $"{indentation}<{currentEntry.Name}";

			if (currentEntry.Attributes.Count > 0)
			{
				fileContent += " ";

				foreach (KeyValuePair<string, string> attribute in currentEntry.Attributes)
				{
					fileContent += $"{attribute.Key}=\"{attribute.Value}\" ";
				}
			}

			if (!string.IsNullOrEmpty(currentEntry.Value))
			{
				fileContent += $">{currentEntry.Value}</{currentEntry.Name}>\r\n";
			}
			else
			{
				if (currentEntry.Children.Count > 0)
				{
					fileContent += ">\r\n";
					foreach (ProjectEntry child in currentEntry.Children)
					{
						WriteTreeToBuffer(child, ref fileContent, indentation + '\t');
					}
					fileContent += $"{indentation}</{currentEntry.Name}>\r\n";
				}
				else
				{
					fileContent += "/>\r\n";
				}
			}
		}
	}
}