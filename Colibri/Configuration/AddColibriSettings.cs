namespace Colibri.Configuration;

internal static class AddColibriSettingsExtension
{
    public static void AddColibriSettings(this IServiceCollection services)
    {
        services
            .AddOptions<ColibriSettings>()
            .BindConfiguration("Colibri:Routing")
            .Validate(settings =>
                    ConfigValidator.Validate(settings),
                "Invalid Settings configuration")
            .ValidateOnStart();
    }
}