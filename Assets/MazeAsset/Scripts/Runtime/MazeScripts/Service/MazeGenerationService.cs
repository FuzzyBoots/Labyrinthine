
namespace MazeAsset.MazeGenerator
{
    internal class MazeGenerationService : IMazeGenerationService
    {
        private IMazeGenerationStrategy _generationStrategy;
        private ICell _cellShape;
        internal MazeData MazeData { get; private set; }

        MazeData IMazeGenerationService.MazeData => MazeData;

        internal MazeGenerationService(IMazeGenerationStrategy generationStrategy, ICell cellShape)
        {
            _generationStrategy = generationStrategy;
            _cellShape = cellShape;
        }

        bool IMazeGenerationService.Initialize()
        {
            MazeData = _generationStrategy.InitializeData();
            var checker = new CheckerConector();
            checker.CheckIfMazeConnected(MazeData, _cellShape, true);
            return MazeData != null;
        }

        void IMazeGenerationService.Generate(bool removeWalls, bool generateFullMaze)
        {
            MazeData.floors = _cellShape.GetWallsList(MazeData.mazeArray, removeWalls);
        }
    }
}
