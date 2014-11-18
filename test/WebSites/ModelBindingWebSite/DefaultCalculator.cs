// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace ModelBindingWebSite
{
    public class DefaultCalculator : ICalculator
    {
        public int Operation(char @operator, int left, int right)
        {
            switch (@operator)
            {
                case '+': return left + right;
                case '-': return left - right;
                case '*': return left * right;
                case '/': return left / right;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}