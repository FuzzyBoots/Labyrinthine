using System;
using UnityEngine;
namespace MazeAsset.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class LabeledVector2Attribute : PropertyAttribute
    {
        public string XLabel { get; }
        public string YLabel { get; }

        public LabeledVector2Attribute(string xLabel, string yLabel)
        {
            XLabel = xLabel;
            YLabel = yLabel;
        }
    }
}
