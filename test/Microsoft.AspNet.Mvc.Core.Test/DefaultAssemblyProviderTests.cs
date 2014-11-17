// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Runtime;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Core
{
    public class DefaultAssemblyProviderTests
    {
        [Fact]
        public void CandidateAssemblies_IgnoresMvcAssemblies()
        {
            // Arrange
            var manager = new Mock<ILibraryManager>();
            manager.Setup(f => f.GetReferencingLibraries(It.IsAny<string>()))
                   .Returns(new[]
                   {
                        CreateLibraryInfo("Microsoft.AspNet.Mvc.Core"),
                        CreateLibraryInfo("Microsoft.AspNet.Mvc"),
                        CreateLibraryInfo("Microsoft.AspNet.Mvc.ModelBinding"),
                        CreateLibraryInfo("SomeRandomAssembly"),
                   })
                   .Verifiable();
            var provider = new DefaultAssemblyProvider(manager.Object);

            // Act
            var candidates = provider.GetCandidateLibraries();

            // Assert
            Assert.Equal(new[] { "SomeRandomAssembly" }, candidates.Select(a => a.Name));
        }

        [Fact]
        public void CandidateAssemblies_ReturnsLibrariesReferencingAnyMvcAssembly()
        {
            // Arrange
            var manager = new Mock<ILibraryManager>();
            manager.Setup(f => f.GetReferencingLibraries(It.IsAny<string>()))
                  .Returns(Enumerable.Empty<ILibraryInformation>());
            manager.Setup(f => f.GetReferencingLibraries("Microsoft.AspNet.Mvc.Core"))
                   .Returns(new[] { CreateLibraryInfo("Foo") });
            manager.Setup(f => f.GetReferencingLibraries("Microsoft.AspNet.Mvc.ModelBinding"))
                   .Returns(new[] { CreateLibraryInfo("Bar") });
            manager.Setup(f => f.GetReferencingLibraries("Microsoft.AspNet.Mvc"))
                   .Returns(new[] { CreateLibraryInfo("Baz") });
            var provider = new DefaultAssemblyProvider(manager.Object);

            // Act
            var candidates = provider.GetCandidateLibraries();

            // Assert
            Assert.Equal(new[] { "Baz", "Foo", "Bar" }, candidates.Select(a => a.Name));
        }

        [Fact]
        public void CandidateAssemblies_ReturnsLibrariesReferencingOverriddenAssemblies()
        {
            // Arrange
            var manager = new Mock<ILibraryManager>();
            manager.Setup(f => f.GetReferencingLibraries(It.IsAny<string>()))
                  .Returns(Enumerable.Empty<ILibraryInformation>());
            manager.Setup(f => f.GetReferencingLibraries("Microsoft.AspNet.Mvc.Core"))
                   .Returns(new[] { CreateLibraryInfo("Baz") });
            manager.Setup(f => f.GetReferencingLibraries("MyAssembly"))
                   .Returns(new[] { CreateLibraryInfo("Foo") });
            manager.Setup(f => f.GetReferencingLibraries("AnotherAssembly"))
                   .Returns(new[] { CreateLibraryInfo("Bar") });
            var defaultProvider = new DefaultAssemblyProvider(manager.Object);
            var overriddenProvider = new TestAssemblyProvider(manager.Object);
            var nullProvider = new NullAssemblyProvider(manager.Object);

            // Act
            var defaultProviderCandidates = defaultProvider.GetCandidateLibraries();
            var overriddenProviderCandidates = overriddenProvider.GetCandidateLibraries();
            var nullProviderCandidates = nullProvider.GetCandidateLibraries();

            // Assert
            Assert.Equal(new[] { "Baz" }, defaultProviderCandidates.Select(a => a.Name));
            Assert.Equal(new[] { "Foo", "Bar" }, overriddenProviderCandidates.Select(a => a.Name));
            Assert.Equal(Enumerable.Empty<string>(), nullProviderCandidates.Select(a => a.Name));
        }

        private static ILibraryInformation CreateLibraryInfo(string name)
        {
            var info = new Mock<ILibraryInformation>();
            info.SetupGet(b => b.Name).Returns(name);
            return info.Object;
        }

        private class TestAssemblyProvider : DefaultAssemblyProvider
        {
            protected override HashSet<string> ReferenceAssemblies
            {
                get
                {
                    return new HashSet<string>
                {
                    "MyAssembly",
                    "AnotherAssembly"
                };
                }
            }

            public TestAssemblyProvider(ILibraryManager libraryManager) : base(libraryManager)
            {
            }
        }

        private class NullAssemblyProvider : DefaultAssemblyProvider
        {
            protected override HashSet<string> ReferenceAssemblies
            {
                get
                {
                    return null;
                }
            }

            public NullAssemblyProvider(ILibraryManager libraryManager) : base(libraryManager)
            {
            }
        }
    }
}