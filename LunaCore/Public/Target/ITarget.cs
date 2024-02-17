namespace Luna.Core.Target
{
    public interface ITarget
    {
        string Name { get; }

        string SolutionPath { get; set; }

        void Register();

        void GenerateSolution();
    }
}
