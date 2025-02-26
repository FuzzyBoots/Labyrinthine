namespace MazeAsset.MazeGenerator
{
    [System.Serializable]
    public class InteractionElevatorTextController
    {

        internal string GetInteractionText(DirectionMoveEnum direction)
        {
            string currentText;
            switch (direction)
            {
                case DirectionMoveEnum.None:
                    currentText = string.Empty;
                    break;
                case DirectionMoveEnum.Up:
                    currentText = "Press Q to move up";
                    break;
                case DirectionMoveEnum.Down:
                    currentText = "Press R to move down";
                    break;
                case DirectionMoveEnum.Both:
                    currentText = "Press Q to move up or R to move down";
                    break;
                default:
                    currentText = string.Empty;
                    break;
            }
            return currentText;
        }
    }
}
