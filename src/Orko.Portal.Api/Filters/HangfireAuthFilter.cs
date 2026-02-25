using Hangfire.Dashboard;

namespace Orko.Portal.Api.Filters;

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Hangfire dashboard'a erişim izni
        // Nginx arkasında olduğu için ve /hangfire public path'te olduğu için
        // erişim kontrolü Nginx seviyesinde yapılabilir
        return true;
    }
}
