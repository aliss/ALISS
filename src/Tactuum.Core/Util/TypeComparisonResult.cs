namespace Tactuum.Core.Util
{
    public class TypeComparisonResult
    {
        public string Name { get; set; }
        public string OldValueText { get; set; }
        public string NewValueText { get; set; }
        public bool IsDifferent
        {
            get
            {
                return OldValueText != NewValueText;
            }
        }
    }
}
