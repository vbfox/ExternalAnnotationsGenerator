using System.Xml.Linq;

namespace ExternalAnnotationsGenerator
{
    public class NugetSpec
    {
        public NugetSpec(string id, 
            string version = "1.0.0.0", 
            string title = "Annotations", 
            string authors = "Anonymous", 
            string owners = "Anonymous",
            string projectUrl = "",
            string iconUrl = "", 
            string description = "", 
            string tags = "")
        {
            Id = id;
            Version = version;
            Title = title;
            Authors = authors;
            Owners = owners;
            ProjectUrl = projectUrl;
            IconUrl = iconUrl;
            Description = description;
            Tags = tags;
        }

        public string Id { get; }
        public string Version { get; }
        public string Title { get; }
        public string Authors { get; }
        public string Owners { get; }
        public string ProjectUrl { get; }
        public string IconUrl { get; }
        public string Description { get; }
        public string Tags { get; }

        internal XDocument GetXml()
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),                
                new XElement("package",
                    new XAttribute("xmlns", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"),
                    new XElement("metadata",
                        new XElement("id", Id),
                        new XElement("version", Version),
                        new XElement("title", Title),
                        new XElement("authors", Authors),
                        new XElement("owners", Owners),
                        new XElement("projectUrl", ProjectUrl),
                        new XElement("iconUrl", IconUrl),
                        new XElement("description", Description),
                        new XElement("tags", Tags),
                        new XElement("dependencies",
                            new XElement("dependency",
                                new XAttribute("id", "Wave"),
                                new XAttribute("version", "[1.0,]")))
                        )));

            return doc;
        }
    }
}