namespace Orc.NuGetExplorer.Controls.Helpers
{
    using System.Text;
    using System.Windows.Markup;
    using System.Xml;

    public class XamlExportHelper
    {
        //useful for extracting control templates from ui elements
        public static string Save(object element)
        {
            // Create an XmlWriter
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                NewLineOnAttributes = true
            };

            XmlWriter writer = XmlWriter.Create(sb, xmlSettings);

            XamlWriter.Save(element, writer);

            return sb.ToString();
        }
    }
}
