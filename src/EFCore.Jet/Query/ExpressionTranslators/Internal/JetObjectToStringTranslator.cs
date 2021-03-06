﻿// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityFrameworkCore.Jet.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.Jet.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class JetObjectToStringTranslator : IMethodCallTranslator
    {
        private const int DefaultLength = 100;

        private static readonly Dictionary<Type, string> _typeMapping
            = new Dictionary<Type, string>
            {
                { typeof(int), "VARCHAR(11)" },
                { typeof(long), "VARCHAR(20)" },
                { typeof(DateTime), $"VARCHAR({DefaultLength})" },
                { typeof(Guid), "VARCHAR(36)" },
                { typeof(bool), "VARCHAR(5)" },
                { typeof(byte), "VARCHAR(3)" },
                { typeof(byte[]), $"VARCHAR({DefaultLength})" },
                { typeof(double), $"VARCHAR({DefaultLength})" },
                { typeof(DateTimeOffset), $"VARCHAR({DefaultLength})" },
                { typeof(char), "VARCHAR(1)" },
                { typeof(short), "VARCHAR(6)" },
                { typeof(float), $"VARCHAR({DefaultLength})" },
                { typeof(decimal), $"VARCHAR({DefaultLength})" },
                { typeof(TimeSpan), $"VARCHAR({DefaultLength})" },
                { typeof(uint), "VARCHAR(10)" },
                { typeof(ushort), "VARCHAR(5)" },
                { typeof(ulong), "VARCHAR(19)" },
                { typeof(sbyte), "VARCHAR(4)" }
            };

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            string storeType;

            if (methodCallExpression.Method.Name == nameof(ToString)
                && methodCallExpression.Arguments.Count == 0
                && methodCallExpression.Object != null
                && _typeMapping.TryGetValue(
                    methodCallExpression.Object.Type
                        .UnwrapNullableType()
                        .UnwrapEnumType(),
                    out storeType))
            {

                return new NullCheckedConvertSqlFunctionExpression(
                    functionName: "CStr",
                    returnType: methodCallExpression.Type,
                    expression: methodCallExpression.Object
                    );
            }

            return null;
        }
    }
}
