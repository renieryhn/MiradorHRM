namespace PlanillaPM.Servicio
{
    //public class SetAuditFields
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public SetAuditFields(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //    }
    //    public class BaseEntity
    //    {
    //        public DateTime FechaCreacion { get; set; }
    //        public DateTime FechaModificacion { get; set; }
    //        public string? CreadoPor { get; set; }
    //        public string? ModificadoPor { get; set; }
    //    }

    //    public void UpdateCommonProperties<T>(object entity, bool bNewRecord)
    //    {
    //        var now = DateTime.UtcNow;
    //        var httpContext = _httpContextAccessor.HttpContext;
    //        var CurrentUser = httpContext?.Session.GetString("MyUserName");

    //        if (bNewRecord)
    //        {
    //            entity.FechaCreacion = now;
    //            entity.CreadoPor = CurrentUser;
    //            entity.FechaModificacion = now;
    //            entity.ModificadoPor = CurrentUser;
    //        }
    //        else
    //        {
    //            entity.FechaModificacion = now;
    //            entity.ModificadoPor = CurrentUser;
    //        }
    //    }
    //}
}
