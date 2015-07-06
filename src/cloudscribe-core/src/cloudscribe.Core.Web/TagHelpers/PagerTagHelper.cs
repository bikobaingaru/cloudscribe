﻿// Author:					Joe Audette
// Created:					2015-07-02
// Last Modified:			2015-07-06
// 

using cloudscribe.Core.Web.ViewModels;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace cloudscribe.Core.Web.TagHelpers
{
    [TargetElement("cs-pager", Attributes = PagingInfoAttributeName)]
    public class PagerTagHelper : TagHelper
    {
        private const string PagingInfoAttributeName = "cs-paging-info";
        private const string AjaxTargetAttributeName = "cs-ajax-target";
        private const string AjaxModeAttributeName = "cs-ajax-mode";
        private const string PageNumberParamAttributeName = "cs-pagenumber-param";

        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string FragmentAttributeName = "asp-fragment";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";

       
        public PagerTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        protected IHtmlGenerator Generator { get; }

        [HtmlAttributeName(PagingInfoAttributeName)]
        public PagingInfo PagingModel { get; set; }
        
        [HtmlAttributeName(AjaxTargetAttributeName)]
        public string AjaxTarget { get; set; } = string.Empty;

        [HtmlAttributeName(AjaxModeAttributeName)]
        public string AjaxMode { get; set; } = "replace";

        [HtmlAttributeName(PageNumberParamAttributeName)]
        public string PageNumberParam { get; set; } = "pageNumber";


        /// <summary>
        /// The name of the action method.
        /// </summary>
        /// <remarks>Must be <c>null</c> if <see cref="Route"/> is non-<c>null</c>.</remarks>
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }

        /// <summary>
        /// The name of the controller.
        /// </summary>
        /// <remarks>Must be <c>null</c> if <see cref="Route"/> is non-<c>null</c>.</remarks>
        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }

        /// <summary>
        /// The protocol for the URL, such as &quot;http&quot; or &quot;https&quot;.
        /// </summary>
        [HtmlAttributeName(ProtocolAttributeName)]
        public string Protocol { get; set; }

        /// <summary>
        /// The host name.
        /// </summary>
        [HtmlAttributeName(HostAttributeName)]
        public string Host { get; set; }

        /// <summary>
        /// The URL fragment name.
        /// </summary>
        [HtmlAttributeName(FragmentAttributeName)]
        public string Fragment { get; set; }

        /// <summary>
        /// Name of the route.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Action"/> or <see cref="Controller"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(RouteAttributeName)]
        public string Route { get; set; }


        /// <summary>
        /// Additional parameters for the route.
        /// </summary>
        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues
        { get; set; }
        = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
          
            if(PagingModel == null) 
            {
                output.SuppressOutput();
                return;
            }
            int totalPages = (int)Math.Ceiling(PagingModel.TotalItems / (double)PagingModel.ItemsPerPage);
            // don't render if only 1 page 
            if (totalPages <= 1) 
            {
                output.SuppressOutput();
                return;
            }
            
            //change the bs-pager element into a normal div
            output.TagName = "div";
            
            output.PreContent.SetContent("<ul class=\"pagination\">");

            string querySeparator;

            TagBuilder linkTemplate = GenerateLinkTemplate();

            var items = new StringBuilder();
            for (var i = 1; i <= totalPages; i++)
            {
                var li = new TagBuilder("li");

                var a = new TagBuilder("a");

                
                a.MergeAttributes(linkTemplate.Attributes);
                string href = a.Attributes["href"];
                querySeparator = href.Contains("?") ? "&" : "?";
                a.Attributes["href"] = href + querySeparator + PageNumberParam + "=" + i.ToString();
               
                a.MergeAttribute("title", $"Click to go to page {i}");

                if(AjaxTarget.Length > 0)
                {
                    a.MergeAttribute("data-ajax", "true");
                    a.MergeAttribute("data-ajax-mode", AjaxMode);
                    a.MergeAttribute("data-ajax-update", AjaxTarget);
                }

                a.InnerHtml = i.ToString();

                if (i == PagingModel.CurrentPage)
                {
                    li.AddCssClass("active");
                }
                li.InnerHtml = a.ToString();
                items.AppendLine(li.ToString());
            }
            output.Content.SetContent(items.ToString());
            //output.
            //output.Content.
            // output.Content.Append("<p>Hey</p>");

            output.PostContent.SetContent("</ul>");
            output.Attributes.Clear();
            //output.Attributes.Add("class", "pager");

            //base.Process(context, output);
        }

        private TagBuilder GenerateLinkTemplate()
        {
            var routeValues = RouteValues.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)kvp.Value,
                    StringComparer.OrdinalIgnoreCase);

            TagBuilder tagBuilder;
            if (Route == null)
            {
                tagBuilder = Generator.GenerateActionLink(linkText: string.Empty,
                                                          actionName: Action,
                                                          controllerName: Controller,
                                                          protocol: Protocol,
                                                          hostname: Host,
                                                          fragment: Fragment,
                                                          routeValues: routeValues,
                                                          htmlAttributes: null);
            }
            else if (Action != null || Controller != null)
            {
                // Route and Action or Controller were specified. Can't determine the href attribute.
                throw new InvalidOperationException("not enough info to build pager links");
            }
            else
            {
                tagBuilder = Generator.GenerateRouteLink(linkText: string.Empty,
                                                         routeName: Route,
                                                         protocol: Protocol,
                                                         hostName: Host,
                                                         fragment: Fragment,
                                                         routeValues: routeValues,
                                                         htmlAttributes: null);
            }

            return tagBuilder;
        }


    }
}