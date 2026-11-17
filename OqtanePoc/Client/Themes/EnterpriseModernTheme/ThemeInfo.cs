using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Shared;
using Oqtane.Themes;

namespace OqtanePoc.Themes.EnterpriseModernTheme
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "Enterprise Modern Theme",
            Version = "1.0.0",
            PackageName = "OqtanePoc.Themes.EnterpriseModernTheme",
            Resources = new List<Resource>()
            {
                new Stylesheet(Constants.BootstrapStylesheetUrl, Constants.BootstrapStylesheetIntegrity, "anonymous"),
                new Stylesheet("~/Themes/EnterpriseModernTheme/theme.css"),
                new Script(Constants.BootstrapScriptUrl, Constants.BootstrapScriptIntegrity, "anonymous")
            }
        };
    }
}
