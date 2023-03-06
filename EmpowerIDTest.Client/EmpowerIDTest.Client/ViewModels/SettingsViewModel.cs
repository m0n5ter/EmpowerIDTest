using System;
using System.IO;
using EmpowerIDTest.Client.Utils;
using Newtonsoft.Json.Linq;

namespace EmpowerIDTest.Client.ViewModels;

internal class SettingsViewModel : ViewModelBase
{
    private static readonly string _fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EmpowerIDTest.Client", "settings.json");
    private static readonly Throttler _savingThrottler = new();

    private int _itemsPerPage = 20;
    private string _serviceAddress = string.Empty;

    public string ServiceAddress
    {
        get => _serviceAddress;
        set => SetProperty(ref _serviceAddress, value);
    }

    public int ItemsPerPage
    {
        get => _itemsPerPage;
        set => SetProperty(ref _itemsPerPage, value);
    }

    public static SettingsViewModel Load()
    {
        JObject jObject;

        try
        {
            var json = File.Exists(_fileName) ? File.ReadAllText(_fileName) : "{}";
            jObject = JObject.Parse(json);
        }
        catch
        {
            jObject = new JObject();
        }

        var settings = new SettingsViewModel
        {
            _serviceAddress = jObject[nameof(ServiceAddress)]?.Value<string>() ?? "https://185.51.62.152:8087/api/",
            _itemsPerPage = jObject[nameof(ItemsPerPage)]?.Value<int>() ?? 50
        };

        if (!File.Exists(_fileName))
            settings.Save();

        return settings;
    }

    public void Save()
    {
        _savingThrottler.Next(100, (s, ct) =>
        {
            try
            {
                var jObject = new JObject
                {
                    [nameof(ServiceAddress)] = _serviceAddress,
                    [nameof(ItemsPerPage)] = _itemsPerPage
                };

                var dir = Path.GetDirectoryName(_fileName);

                if (dir != null)
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    File.WriteAllText(_fileName, jObject.ToString());
                }
            }
            catch (Exception e)
            {
                // TODO: Log this?
            }
        });
    }
}