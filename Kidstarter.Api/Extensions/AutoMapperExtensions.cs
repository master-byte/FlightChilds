using System;
using System.Linq.Expressions;

using AutoMapper;

namespace Kidstarter.Api.Extensions
{
    internal static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination, TMember>(
            this IMappingExpression<TSource, TDestination> src,
            Expression<Func<TDestination, TMember>> selector)
        {
            return src.ForMember(selector, opt => opt.Ignore());
        }
    }
}