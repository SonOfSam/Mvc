// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Mvc
{
    [AssemblyNeutral]
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> CandidateAssemblies { get; }
    }
}
