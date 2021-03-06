﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace TagHelpersWebSite.TagHelpers
{
    [HtmlTargetElement("input")]
    public class AddProcessedAttributeTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add(new TagHelperAttribute { Name = "processed", Minimized = true });
        }
    }
}
