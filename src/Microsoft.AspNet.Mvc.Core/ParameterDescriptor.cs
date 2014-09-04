// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Mvc
{
    public class ParameterDescriptor : Descriptor
    {
        public bool IsOptional { get; set; }

        public IUberBinding Binding { get; set; }

        public ParameterBindingInfo ParameterBindingInfo { get; set; }

        public BodyParameterInfo BodyParameterInfo { get; set; }
    }
}
