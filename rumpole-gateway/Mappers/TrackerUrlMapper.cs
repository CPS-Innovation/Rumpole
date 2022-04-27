﻿using System;
using Microsoft.AspNetCore.Http;

namespace RumpoleGateway.Mappers
{
	public class TrackerUrlMapper : ITrackerUrlMapper
	{
		public Uri Map(HttpRequest request)
        {
            var builder = new UriBuilder();
            builder.Scheme = request.Scheme;
            builder.Host = request.Host.Host;
            if (request.Host.Port.HasValue)
            {
                builder.Port = request.Host.Port.Value;
            }
            builder.Path = $"{request.Path}/tracker";

            return builder.Uri;
        }
	}
}

