// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Yarp.ReverseProxy.Service.RuntimeModel.Transforms;

namespace Duende.Bff
{
    /// <summary>
    /// Forwards headers
    /// </summary>
    public class ForwardHeadersTransform : RequestTransform
    {
        private readonly IEnumerable<string> _headerNames;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="headerNames"></param>
        public ForwardHeadersTransform(IEnumerable<string> headerNames)
        {
            _headerNames = headerNames;
        }
        
        /// <inheritdoc />
        public override ValueTask ApplyAsync(RequestTransformContext context)
        {
            foreach (var header in context.HttpContext.Request.Headers)
            {
                var headerName = header.Key;
                var headerValue = header.Value;
                
                if (StringValues.IsNullOrEmpty(headerValue))
                {
                    continue;
                }

                // Filter out HTTP/2 pseudo headers like ":method" and ":path", those go into other fields.
                if (headerName.Length > 0 && headerName[0] == ':')
                {
                    continue;
                }

                if (_headerNames.Contains(headerName, StringComparer.OrdinalIgnoreCase))
                {
                    RequestUtilities.AddHeader(context.ProxyRequest, headerName, headerValue);    
                }
            }

            return default;
        }
    }
}