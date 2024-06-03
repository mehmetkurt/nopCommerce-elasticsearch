using FluentValidation;
using Nop.Plugin.Misc.Elasticsearch.Settings;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Elasticsearch.Validators;
public class ElasticsearchSettingsValidator : BaseNopValidator<ElasticsearchSettings>
{
    public ElasticsearchSettingsValidator()
    {
        When(p => p.Active, () =>
        {
            RuleFor(r => r.Hostnames).NotEmpty().NotNull();

            When(p => p.ConnectionType == (int)ConnectionType.Api, () =>
            {
                RuleFor(r => r.ApiKey).NotEmpty().NotNull();
                RuleFor(r => r.Fingerprint).NotEmpty().NotNull();
            });

            When(p => p.ConnectionType == (int)ConnectionType.Cloud, () =>
            {
                RuleFor(r => r.CloudId).NotEmpty().NotNull();
                RuleFor(r => r.ApiKey).NotEmpty().NotNull();
            });

            When(p => p.ConnectionType == (int)ConnectionType.Basic, () =>
            {
                RuleFor(r => r.Username).NotEmpty().NotNull();
                RuleFor(r => r.Password).NotEmpty().NotNull();
                RuleFor(r => r.Fingerprint).NotEmpty().NotNull().When(w => w.UseFingerprint);
            });
        }).Otherwise(() =>
        {
            RuleFor(r => r.Active).NotEqual(false);
        });
    }
}
