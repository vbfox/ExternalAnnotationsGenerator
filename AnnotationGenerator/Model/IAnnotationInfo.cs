using System.Collections.Generic;
using System.Xml.Linq;

namespace AnnotationGenerator.Model
{
    public interface IAnnotationInfo
    {
        IEnumerable<XElement> GetAttributesXml();
    }
}