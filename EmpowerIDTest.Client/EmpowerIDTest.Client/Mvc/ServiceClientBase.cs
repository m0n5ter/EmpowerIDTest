using System;
using System.Net.Http;
using EmpowerIDTest.Client.ViewModels;

namespace EmpowerIDTest.Client.Mvc;

internal abstract class ServiceClientBase
{
    protected static HttpClient? _httpClient;

    protected ServiceClientBase(SettingsViewModel settings)
    {
        _httpClient ??= new HttpClient(new ProblemDetailsHttpMessageHandler()) {BaseAddress = new Uri(settings.ServiceAddress)};
    }
}