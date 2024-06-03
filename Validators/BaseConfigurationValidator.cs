using FluentValidation;
using Nop.Plugin.SearchProvider.Elasticsearch.Settings;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Validators;

/// <summary>
/// A static class for configuring common validation rules for Elasticsearch settings and configuration models.
/// </summary>
public static class BaseConfigurationValidator
{
    /// <summary>
    /// Configures common validation rules for Elasticsearch settings and configuration models.
    /// </summary>
    /// <typeparam name="T">The type of the configuration to validate.</typeparam>
    /// <param name="validator">The validator instance to configure rules for.</param>
    /// <param name="localizationService">The localization service for retrieving error messages.</param>
    public static void ConfigureCommonRules<T>(AbstractValidator<T> validator, ILocalizationService localizationService) where T : IConfigurationValidator
    {
        // Configure rules only when the configuration is active
        validator.When(p => p.Active, () =>
        {
            validator.RuleFor(r => r.Hostnames)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames.NotEmpty"))
                .NotNull()
                .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Hostnames.NotNull"));

            validator.RuleFor(r => r.ConnectionType)
                .NotEqual(0)
                .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ConnectionType.NotEqual"));

            // Configure rules specific to cloud connection type
            validator.When(p => p.ConnectionType == (int)ConnectionType.Cloud, () =>
            {
                validator.RuleFor(r => r.CloudId)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId.NotEmpty"))
                    .NotNull()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.CloudId.NotNull"));
            });

            // Configure rules specific to basic connection type
            validator.When(p => p.ConnectionType == (int)ConnectionType.Basic, () =>
            {
                validator.RuleFor(r => r.Username)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username.NotEmpty"))
                    .NotNull()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Username.NotNull"));

                validator.RuleFor(r => r.Password)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password.NotEmpty"))
                    .NotNull()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Password.NotNull"));
            });

            // Configure rules specific to API or basic connection types when use fingerprint is enabled
            validator.When(p => p.ConnectionType == (int)ConnectionType.Api || p.ConnectionType == (int)ConnectionType.Basic, () =>
            {
                validator.RuleFor(r => r.Fingerprint)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint.NotEmpty"))
                    .NotNull()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.Fingerprint.NotNull"))
                    .When(w => w.UseFingerprint);
            });

            // Configure rules specific to API or cloud connection types
            validator.When(p => p.ConnectionType == (int)ConnectionType.Api || p.ConnectionType == (int)ConnectionType.Cloud, () =>
            {
                validator.RuleFor(r => r.ApiKey)
                    .NotEmpty()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey.NotEmpty"))
                    .NotNull()
                    .WithMessageAwait(localizationService.GetResourceAsync($"{ElasticsearchDefaults.LocalizationPrefix}.Configuration.ApiKey.NotNull"));
            });
        });
    }
}
