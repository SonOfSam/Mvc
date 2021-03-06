// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Filters
{
    public interface IActionFilter : IFilterMetadata
    {
        void OnActionExecuting([NotNull] ActionExecutingContext context);

        void OnActionExecuted([NotNull] ActionExecutedContext context);
    }
}