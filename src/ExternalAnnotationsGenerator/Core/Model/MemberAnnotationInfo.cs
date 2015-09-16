namespace ExternalAnnotationsGenerator.Core.Model
{
    public class MemberAnnotationInfo
    {
        public bool IsNotNull { get; }
        public bool CanBeNull { get; }

        public MemberAnnotationInfo(bool isNotNull = false, bool canBeNull = false)
        {
            IsNotNull = isNotNull;
            CanBeNull = canBeNull;
        }
    }
}
