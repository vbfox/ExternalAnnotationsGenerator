namespace AnnotationGenerator.Model
{
    internal class MemberAnnotationInfo
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
