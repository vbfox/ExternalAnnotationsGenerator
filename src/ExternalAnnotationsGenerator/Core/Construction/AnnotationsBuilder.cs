using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExternalAnnotationsGenerator.Core.Model;

namespace ExternalAnnotationsGenerator.Core.Construction
{
    public class AnnotationsBuilder : IAnnotator
    {
        readonly List<AssemblyAnnotations> annotations = new List<AssemblyAnnotations>();

        public IEnumerable<AssemblyAnnotations> GetAnnotations()
        {
            return annotations;
        }

        private AssemblyAnnotations GetAssemblyAnnotations(Assembly assembly)
        {
            var existing = annotations.FirstOrDefault(m => m.Assembly == assembly);
            if (existing != null)
            {
                return existing;
            }

            var newAnnotations = new AssemblyAnnotations(assembly);
            annotations.Add(newAnnotations);
            return newAnnotations;
        }

        public void AnnotateType<TType>(Action<ITypeAnnotator<TType>> annotationActions)
        {
            if (annotationActions == null) throw new ArgumentNullException(nameof(annotationActions));

            var memberAnnotator = new TypeAnnotationsBuilder<TType>();
            annotationActions(memberAnnotator);

            var assemblyAnnotations = GetAssemblyAnnotations(typeof (TType).Assembly);
            assemblyAnnotations.AddRange(memberAnnotator.GetMembersAnnotations());
        }

        public void AnnotateStatic(Expression<Action> expression)
        {
            throw new NotImplementedException();
        }

        public void AnnotateStatic<TResult>(Expression<Func<TResult>> expression)
        {
            throw new NotImplementedException();
        }
    }
}