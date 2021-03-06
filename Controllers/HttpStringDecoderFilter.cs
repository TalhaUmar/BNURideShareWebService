﻿using System;
using System.Net;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RideShareWebServices.Controllers
{
    public class HttpStringDecoderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            // If the request is either a PUT or POST, attempt to decode all strings
            if (actionContext.Request.Method.ToString() == System.Net.WebRequestMethods.Http.Post
                || actionContext.Request.Method.ToString() == System.Net.WebRequestMethods.Http.Put)
            {
                // For each of the items in the PUT/POST
                foreach (var item in actionContext.ActionArguments.Values)
                {
                    try
                    {
                        // Get the type of the object
                        Type type = item.GetType();

                        // For each property of this object, html decode it if it is of type string
                        foreach (PropertyInfo propertyInfo in type.GetProperties())
                        {
                            var prop = propertyInfo.GetValue(item);
                            if (prop != null && prop.GetType() == typeof(string))
                            {
                                propertyInfo.SetValue(item, WebUtility.HtmlDecode((string)prop));
                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}