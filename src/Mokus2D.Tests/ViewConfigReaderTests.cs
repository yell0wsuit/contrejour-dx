using System.Collections.Generic;
using System.Xml.Linq;

using Mokus2D.Content;

using Xunit;

namespace Mokus2D.Tests
{
    public class ViewConfigReaderTests
    {
        [Fact]
        public void ReadsEachConfigInAttributeOrder()
        {
            XDocument views = XDocument.Parse("<root><McSky><config noAlpha=\"true\" z=\"-8\" layer=\"4\"/></McSky><McBare/></root>");

            Dictionary<string, Dictionary<string, string>> result = ViewConfigReader.Read(views);

            Assert.Equal(["McSky"], result.Keys);
            Assert.Equal(["noAlpha", "z", "layer"], result["McSky"].Keys);
            Assert.Equal("-8", result["McSky"]["z"]);
        }

        [Fact]
        public void MissingFileMeansNoConfigs()
        {
            Assert.Empty(ViewConfigReader.Read(null));
        }
    }
}
