﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Xunit;

namespace Microsoft.AspNet.Mvc.TagHelpers
{
    public class InputTagHelperTest
    {
        // Model (List<Model> or Model instance), container type (Model or NestModel), model accessor,
        // property path / id, expected value.
        public static TheoryData<object, Type, Func<object>, NameAndId, string> TestDataSet
        {
            get
            {
                var modelWithNull = new Model
                {
                    NestedModel = new NestedModel
                    {
                        Text = null,
                    },
                    Text = null,
                };
                var modelWithText = new Model
                {
                    NestedModel = new NestedModel
                    {
                        Text = "inner text",
                    },
                    Text = "outer text",
                };
                var models = new List<Model>
                {
                    modelWithNull,
                    modelWithText,
                };

                return new TheoryData<object, Type, Func<object>, NameAndId, string>
                {
                    { null, typeof(Model), () => null, new NameAndId("Text", "Text"),
                        string.Empty },

                    { modelWithNull, typeof(Model), () => modelWithNull.Text, new NameAndId("Text", "Text"),
                        string.Empty },
                    { modelWithText, typeof(Model), () => modelWithText.Text, new NameAndId("Text", "Text"),
                        "outer text" },

                    { modelWithNull, typeof(NestedModel), () => modelWithNull.NestedModel.Text,
                        new NameAndId("NestedModel.Text", "NestedModel_Text"), string.Empty },
                    { modelWithText, typeof(NestedModel), () => modelWithText.NestedModel.Text,
                        new NameAndId("NestedModel.Text", "NestedModel_Text"), "inner text" },

                    // Top-level indexing does not work end-to-end due to code generation issue #1345.
                    // TODO: Remove above comment when #1345 is fixed.
                    { models, typeof(Model), () => models[0].Text,
                        new NameAndId("[0].Text", "z0__Text"), string.Empty },
                    { models, typeof(Model), () => models[1].Text,
                        new NameAndId("[1].Text", "z1__Text"), "outer text" },

                    { models, typeof(NestedModel), () => models[0].NestedModel.Text,
                        new NameAndId("[0].NestedModel.Text", "z0__NestedModel_Text"), string.Empty },
                    { models, typeof(NestedModel), () => models[1].NestedModel.Text,
                        new NameAndId("[1].NestedModel.Text", "z1__NestedModel_Text"), "inner text" },
                };
            }
        }

        [Theory]
        [MemberData(nameof(TestDataSet))]
        public async Task ProcessAsync_GeneratesExpectedOutput(
            object model,
            Type containerType,
            Func<object> modelAccessor,
            NameAndId nameAndId,
            string expectedValue)
        {
            // Arrange
            var expectedAttributes = new Dictionary<string, string>
            {
                { "class", "form-control" },
                { "type", "text" },
                { "id", nameAndId.Id },
                { "name", nameAndId.Name },
                { "valid", "from validation attributes" },
                { "value", expectedValue },
            };
            var expectedContent = "original content";
            var expectedTagName = "not-input";

            var metadataProvider = new DataAnnotationsModelMetadataProvider();

            // Property name is either nameof(Model.Text) or nameof(NestedModel.Text).
            var metadata = metadataProvider.GetMetadataForProperty(modelAccessor, containerType, propertyName: "Text");
            var modelExpression = new ModelExpression(nameAndId.Name, metadata);

            var tagHelperContext = new TagHelperContext(new Dictionary<string, object>());
            var htmlAttributes = new Dictionary<string, string>
            {
                { "class", "form-control" },
            };
            var output = new TagHelperOutput(expectedTagName, htmlAttributes, expectedContent)
            {
                SelfClosing = false,
            };

            var htmlGenerator = new TestableHtmlGenerator(metadataProvider)
            {
                ValidationAttributes =
                {
                    {  "valid", "from validation attributes" },
                }
            };
            var viewContext = TestableHtmlGenerator.GetViewContext(model, htmlGenerator, metadataProvider);
            var tagHelper = new InputTagHelper
            {
                Generator = htmlGenerator,
                For = modelExpression,
                ViewContext = viewContext,
            };

            // Act
            await tagHelper.ProcessAsync(tagHelperContext, output);

            // Assert
            Assert.Equal(expectedAttributes, output.Attributes);
            Assert.Equal(expectedContent, output.Content);
            Assert.True(output.SelfClosing);
            Assert.Equal(expectedTagName, output.TagName);
        }

        [Fact]
        public async Task TagHelper_RestoresTypeAndValue_IfForNotBound()
        {
            // Arrange
            var expectedAttributes = new Dictionary<string, string>
            {
                { "class", "form-control" },
                { "type", "datetime" },
                { "value", "2014-10-15T23:24:19.000-7:00" },
            };
            var expectedContent = "original content";
            var expectedTagName = "input";

            var metadataProvider = new DataAnnotationsModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForProperty(
                modelAccessor: () => null,
                containerType: typeof(Model),
                propertyName: nameof(Model.Text));
            var modelExpression = new ModelExpression(nameof(Model.Text), metadata);

            var tagHelperContext = new TagHelperContext(new Dictionary<string, object>());
            var output = new TagHelperOutput(expectedTagName, expectedAttributes, expectedContent)
            {
                SelfClosing = false,
            };

            var htmlGenerator = new TestableHtmlGenerator(metadataProvider)
            {
                ValidationAttributes =
                {
                    {  "valid", "from validation attributes" },
                }
            };
            Model model = null;
            var viewContext = TestableHtmlGenerator.GetViewContext(model, htmlGenerator, metadataProvider);
            var tagHelper = new InputTagHelper
            {
                Generator = htmlGenerator,
                ViewContext = viewContext,
            };

            // Act
            await tagHelper.ProcessAsync(tagHelperContext, output);

            // Assert
            Assert.Equal(expectedAttributes, output.Attributes);
            Assert.Equal(expectedContent, output.Content);
            Assert.False(output.SelfClosing);
            Assert.Equal(expectedTagName, output.TagName);
        }

        public class NameAndId
        {
            public NameAndId(string name, string id)
            {
                Name = name;
                Id = id;
            }

            public string Name { get; private set; }

            public string Id { get; private set; }
        }

        private class Model
        {
            public string Text { get; set; }

            public NestedModel NestedModel { get; set; }
        }

        private class NestedModel
        {
            public string Text { get; set; }
        }
    }
}
