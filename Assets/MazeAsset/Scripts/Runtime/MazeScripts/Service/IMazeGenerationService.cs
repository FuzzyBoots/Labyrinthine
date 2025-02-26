namespace MazeAsset.MazeGenerator
{
    internal interface IMazeGenerationService
    {
        internal MazeData MazeData { get; }
        internal bool Initialize();
        internal void Generate(bool removeWalls, bool generateFullMaze);
    }
}
