using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Utilities.HtmlHelpers;
public static class HtmlHelpers
{
    public static string GetPropertyDescription<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, System.Linq.Expressions.Expression<Func<TModel, TProperty>> expression)
    {
        var member = expression.Body as System.Linq.Expressions.MemberExpression;
        if (member == null) return string.Empty;

        var attribute = member.Member.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? string.Empty;
    }
}
