using UnityEngine;
namespace MazeAsset.CustomAttribute
{
    public class VisibleIfAttribute : PropertyAttribute
    {
        public string conditionFieldName { get; set; }
        public bool compare { get; set; }

        public VisibleIfAttribute(string conditionFieldName, bool compare = true)
        {
            this.conditionFieldName = conditionFieldName;
            this.compare = compare;
        }
    }
}