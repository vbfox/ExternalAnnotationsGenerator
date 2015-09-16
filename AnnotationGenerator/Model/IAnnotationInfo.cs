using System.Collections.Generic;
using System.Xml.Linq;

namespace AnnotationGenerator.Model
{
    internal interface IAnnotationInfo
    {
        IEnumerable<XElement> GetAttributesXml();
    }
}