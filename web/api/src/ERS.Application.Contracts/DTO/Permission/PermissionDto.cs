using System.Collections.Generic;
namespace ERS.DTO
{
    public class PermissionDto
    {
    }
    public class RoleResult
    {
        public string company { get; set; }
        public string role { get; set; }
        public string roleId { get; set; }
        public string roleName { get; set; }
        public string userId { get; set; }
        public string SubjectId { get; set; }
        public string userName { get; set; }
    }
    public class SelectParams
    {
        public string company { get; set; }
        public string role { get; set; }
        public IEnumerable<string> emplid { get; set; }
    }
    public class category
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public class UserAcl
    {
        public string PermissionRequired { get; set; }
        public string UserId { get; set; }
        public string DataScope { get; set; }
        public string Program { get; set; }
    }
    public class RolesList
    {
        public IEnumerable<Role> roles { get; set; }
    }
    public class Role
    {
        public string name { get; set; }
        public string description { get; set; }
        public string id { get; set; }
    }
    public class RoleData
    {
        public string userId { get; set; }
    }
}
