// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace ModelBindingWebSite.Controllers
{
    public class CalculatorController : Controller
    {
        public int Calculate(CalculatorContext context)
        {
            return context.Calculator.Operation(context.Operator, context.Left, context.Right);
        }

        public int Add(int left, int right, [FromServices] ICalculator calculator)
        {
            return calculator.Operation('+', left, right);
        }
    }
}