using Luna.Core;

namespace Luna.Targets.VisualStudio.Projects.Cpp
{
	public class Project(string name, string relativePath, Guid guid)
		: BaseProject(BaseProject.ProjectToGuid(VisualStudioProjectType.Cpp), name, relativePath, guid)
	{
		public override string Extension => "vxproj";

		public override bool WriteFile()
		{
			ILogService? logService = ServiceProvider.LogService;

			logService?.Log($"Writing Project: {RelativePath}\\{Name}\\{Name}.{Extension}");
			logService?.Log($"Writing Project Done.");
			return true;
		}
	}
}