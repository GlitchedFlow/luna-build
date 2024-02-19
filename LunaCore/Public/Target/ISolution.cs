namespace Luna.Core.Target
{
	public interface ISolution
	{
		public bool AddProject(IProject project);

		public bool WriteFile();
	}
}