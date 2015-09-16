using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator.Core.Model
{
    public class AssemblyAnnotations : IEnumerable<MemberAnnotations>
    {
        public Assembly Assembly { get; }

        private readonly List<MemberAnnotations> membersAnnotations = new List<MemberAnnotations>();

        public AssemblyAnnotations([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Assembly = assembly;
        }

        public void AddRange([NotNull] IEnumerable<MemberAnnotations> memberAnnotations)
        {
            if (memberAnnotations == null) throw new ArgumentNullException(nameof(memberAnnotations));

            membersAnnotations.AddRange(memberAnnotations);
        }

        public IEnumerator<MemberAnnotations> GetEnumerator()
        {
            return membersAnnotations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return membersAnnotations.GetEnumerator();
        }
    }
}