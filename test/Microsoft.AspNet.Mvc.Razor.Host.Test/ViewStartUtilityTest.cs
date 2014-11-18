// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Microsoft.AspNet.FileSystems;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Infrastructure;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class ViewStartProviderTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetViewStartLocations_ReturnsEmptySequenceIfViewPathIsEmpty(string viewPath)
        {
            // Act
            var result = ViewStartUtility.GetViewStartLocations(new TestFileSystem(), viewPath);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("/Views/Home/MyView.cshtml")]
        [InlineData("~/Views/Home/MyView.cshtml")]
        [InlineData("Views/Home/MyView.cshtml")]
        public void GetViewStartLocations_ReturnsPotentialViewStartLocations(string inputPath)
        {
            // Arrange
            var expected = new[]
            {
                @"Views\Home\_ViewStart.cshtml",
                @"Views\_ViewStart.cshtml",
                @"_ViewStart.cshtml"
            };
            var fileSystem = new PhysicalFileSystem(GetTestFileSystemBase());

            // Act
            var result = ViewStartUtility.GetViewStartLocations(fileSystem, inputPath);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("/Views/Home/_ViewStart.cshtml")]
        [InlineData("~/Views/Home/_Viewstart.cshtml")]
        [InlineData("Views/Home/_Viewstart.cshtml")]
        public void GetViewStartLocations_SkipsCurrentPath_IfCurrentIsViewStart(string inputPath)
        {
            // Arrange
            var expected = new[]
            {
                @"Views\_ViewStart.cshtml",
                @"_ViewStart.cshtml"
            };
            var fileSystem = new PhysicalFileSystem(GetTestFileSystemBase());

            // Act
            var result = ViewStartUtility.GetViewStartLocations(fileSystem, inputPath);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Test.cshtml")]
        [InlineData("ViewStart.cshtml")]
        public void GetViewStartLocations_ReturnsPotentialViewStartLocations_IfPathIsAbsolute(string fileName)
        {
            // Arrange
            var expected = new[]
            {
                @"Areas\MyArea\Sub\Views\Admin\_ViewStart.cshtml",
                @"Areas\MyArea\Sub\Views\_ViewStart.cshtml",
                @"Areas\MyArea\Sub\_ViewStart.cshtml",
                @"Areas\MyArea\_ViewStart.cshtml",
                @"Areas\_ViewStart.cshtml",
                @"_ViewStart.cshtml",
            };
            var appBase = GetTestFileSystemBase();
            var viewPath = Path.Combine(appBase, "Areas", "MyArea", "Sub", "Views", "Admin", fileName);
            var fileSystem = new PhysicalFileSystem(appBase);

            // Act
            var result = ViewStartUtility.GetViewStartLocations(fileSystem, viewPath);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("_ViewStart.cshtml")]
        [InlineData("_viewstart.cshtml")]
        public void GetViewStartLocations_SkipsCurrentPath_IfAbsolutePathIsAViewStartFile(string fileName)
        {
            // Arrange
            var expected = new[]
            {
                @"Areas\MyArea\Sub\Views\_ViewStart.cshtml",
                @"Areas\MyArea\Sub\_ViewStart.cshtml",
                @"Areas\MyArea\_ViewStart.cshtml",
                @"Areas\_ViewStart.cshtml",
                @"_ViewStart.cshtml",
            };
            var appBase = GetTestFileSystemBase();
            var viewPath = Path.Combine(appBase, "Areas", "MyArea", "Sub", "Views", "Admin", fileName);
            var fileSystem = new PhysicalFileSystem(appBase);

            // Act
            var result = ViewStartUtility.GetViewStartLocations(fileSystem, viewPath);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetViewStartLocations_ReturnsEmptySequence_IfViewStartIsAtRoot()
        {
            // Arrange
            var appBase = GetTestFileSystemBase();
            var viewPath = "_ViewStart.cshtml";
            var fileSystem = new PhysicalFileSystem(appBase);

            // Act
            var result = ViewStartUtility.GetViewStartLocations(fileSystem, viewPath);

            // Assert
            Assert.Empty(result);
        }

        private static string GetTestFileSystemBase()
        {
            var serviceProvider = CallContextServiceLocator.Locator.ServiceProvider;
            var appEnv = (IApplicationEnvironment)serviceProvider.GetService(typeof(IApplicationEnvironment));
            return Path.Combine(appEnv.ApplicationBasePath, "TestFiles", "ViewStartUtilityFiles");
        }
    }
}