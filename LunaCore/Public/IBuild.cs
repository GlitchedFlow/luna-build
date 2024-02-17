using Luna.Core.Target;

namespace Luna.Core
{
	public interface IBuild
	{
		public void Register();
		public void Configurate();
		public void Generate();
	}
}
