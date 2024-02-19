using Luna.Core.Target;

namespace Luna.Core
{
	public interface IGeneratorService : IMeta
	{
		public ITarget? ActiveTarget { get; set; }

		public bool Generate();
	}
}