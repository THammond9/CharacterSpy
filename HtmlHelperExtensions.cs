using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace CharacterSpy.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString CharacterCountOf<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Func<Expression<Func<TModel, TProperty>>, object, IHtmlString> helperMethod,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            var methodResult = helperMethod(expression, htmlAttributes);
            var markedResult = methodResult.MarkTarget();
            return BuildCharacterWatcher(markedResult.Item1, expression, markedResult.Item2);
        }

        public static IHtmlString CharacterCountOf<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Func<Expression<Func<TModel, TProperty>>, object, string, IHtmlString> helperMethod,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes,
            string optionalParameterOne = null)
        {
            var methodResult = helperMethod(expression, htmlAttributes, optionalParameterOne);
            var markedResult = methodResult.MarkTarget();
            return BuildCharacterWatcher(markedResult.Item1, expression, markedResult.Item2);
        }

        private static Tuple<IHtmlString, string> MarkTarget(this IHtmlString generatedHtmlTag)
        {
            var targetIdentifier = Guid.NewGuid().ToString();
            var tagString = generatedHtmlTag.ToString();
            var markedString = tagString.Insert(tagString.IndexOf(" ", StringComparison.CurrentCulture), $" {targetIdentifier} ");
            var markedTag = new HtmlString(markedString);
            return new Tuple<IHtmlString, string>(markedTag, targetIdentifier);
        }

        private static IHtmlString BuildCharacterWatcher<TModel, TProperty>(
            IHtmlString methodResult,
            Expression<Func<TModel, TProperty>> expression,
            string targetIdentifier)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                return methodResult;
            }

            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var attribute = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
            if (attribute == null || attribute.MaximumLength <= 5)
            {
                return methodResult;
            }

            var watchId = Guid.NewGuid().ToString();

            var tag1 = new TagBuilder("span");
            tag1.Attributes.Add("class", "text-content-max-length-countdown");
            tag1.Attributes.Add("id", watchId);

            var tag2 = new TagBuilder("script");
            tag2.Attributes.Add("type", "text/javascript");
            tag2.InnerHtml =
                $"$(function(){{initCountdownWatcher('[{targetIdentifier}]', '#{watchId}', {attribute.MaximumLength});}});";

            return new HtmlString(methodResult.ToString() + tag1 + tag2);
        }
    }
}