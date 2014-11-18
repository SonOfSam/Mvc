// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents an <see cref="IModelBinder"/> which understands <see cref="IServiceActivatorBinderMetadata"/>
    /// and activates a given model using <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServicesModelBinder : MetadataAwareBinder<IServiceActivatorBinderMetadata>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new instance of <see cref="ServicesModelBinder"/>.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ServicesModelBinder([NotNull] IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task<bool> BindAsync(
            ModelBindingContext bindingContext,
            IServiceActivatorBinderMetadata metadata)
        {
            bindingContext.Model = _serviceProvider.GetService(bindingContext.ModelType);
            return Task.FromResult(true);
        }
    }
}
