using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

using Mokus2D.Content;

using Xunit;

namespace Mokus2D.Tests
{
    public class ResourcesLoaderFallbackTests
    {
        // Throws the given exception for the 2x load and returns "1x" for the fallback.
        private sealed class FakeLoader(Exception doubledError) : ResourcesLoaderBase
        {
            protected override string GetFileName<T>(string resourceName, string resourceSuffix = null)
            {
                throw new NotSupportedException();
            }

            protected override T ProcessXml<T>(string name, XDocument xml)
            {
                throw new NotSupportedException();
            }

            protected override T LoadData<T>(string name, string resourcesSuffix)
            {
                return resourcesSuffix != null ? throw doubledError : (T)(object)"1x";
            }
        }

        private static string Load(Exception doubledError)
        {
            FakeLoader loader = new(doubledError) { FallbackToDefaultScaleFactor = true, PrefferedScaleFactor = 0.5f };
            return loader.Load<string>("common/McFoo");
        }

        [Fact]
        public void MissingFileOrNameFallsBackToOneX()
        {
            Assert.Equal("1x", Load(new FileNotFoundException()));
            Assert.Equal("1x", Load(new KeyNotFoundException()));
#pragma warning disable CA2201 // Reproduces the legacy missing-XML fallback path.
            Assert.Equal("1x", Load(new NullReferenceException()));
#pragma warning restore CA2201
        }

        [Fact]
        public void BrokenAtlasDataIsNotHiddenByTheFallback()
        {
            _ = Assert.Throws<InvalidDataException>(() => Load(new InvalidDataException()));
            _ = Assert.Throws<JsonException>(() => Load(new JsonException()));
        }
    }
}
