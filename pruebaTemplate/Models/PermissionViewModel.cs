namespace PlanillaPM.Models
{
    public class PermissionViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleClaimsViewModel> AssignedPermissions { get; set; }
        public List<RoleClaimsViewModel> UnassignedPermissions { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }

    public class RoleClaimsViewModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
    public class RoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
