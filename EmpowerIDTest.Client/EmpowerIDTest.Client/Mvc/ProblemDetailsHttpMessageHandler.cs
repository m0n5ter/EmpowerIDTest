using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmpowerIDTest.Client.Mvc;

internal class ProblemDetailsHttpMessageHandler : DelegatingHandler
{
    public ProblemDetailsHttpMessageHandler() : base(new HttpClientHandler
    {
        // Disables certificate validation, remove in production
        ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
    })
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var response = await base.SendAsync(request, ct);
        var mediaType = response.Content.Headers.ContentType?.MediaType;

        if (mediaType != null && mediaType.Equals("application/problem+json", StringComparison.InvariantCultureIgnoreCase))
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>((JsonSerializerOptions?) null, ct) ?? new ProblemDetails();
            throw new HttpRequestException(problemDetails.Detail ?? response.ReasonPhrase);
        }

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(response.ReasonPhrase, null, response.StatusCode);

        return response;
    }
}