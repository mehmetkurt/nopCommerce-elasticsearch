using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;

namespace Nop.Plugin.SearchProvider.Elasticsearch.Infrastructure;

/// <summary>
/// Expands view locations for themes in the Elasticsearch search provider plugin.
/// </summary>
public class ThemeViewLocationExpander : IViewLocationExpander
{
    /// <summary>
    /// Key used to store the theme name in the context values for view location expanding.
    /// </summary>
    protected const string THEME_KEY = "nop.themename";

    /// <summary>
    /// Populates the theme name value based on the current context.
    /// </summary>
    /// <param name="context">The context for the view location expander.</param>
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        if (context.AreaName?.Equals(AreaNames.ADMIN) ?? false)
            return;

        context.Values[THEME_KEY] = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync().Result;
    }

    /// <summary>
    /// Expands the view locations based on the current context.
    /// </summary>
    /// <param name="context">The context for the view location expander.</param>
    /// <param name="viewLocations">The original view locations to expand.</param>
    /// <returns>The expanded view locations.</returns>
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName?.Equals(AreaNames.ADMIN) ?? false)
        {
            viewLocations = new[] {
                $"/Plugins/SearchProvider.Elasticsearch/Areas/{{2}}/Views/{{0}}.cshtml",
                $"/Plugins/SearchProvider.Elasticsearch/Areas/{{2}}/Views/{{1}}/{{0}}.cshtml",
                $"/Plugins/SearchProvider.Elasticsearch/Areas/{{2}}/Views/Shared/{{0}}.cshtml",
            }
            .Concat(viewLocations);
        }

        return viewLocations;
    }
}