using System;
using System.Linq;

namespace RumpoleGateway.Domain.Authorisation
{
    public class User
    {
        public string DisplayName { get; set; }

        public string UserPrincipalName { get; set; }

        //public Role[] Roles { get; set; }

        //public AreaDivision[] AreaDivisions { get; set; }

        //public bool hasRole(Role role)
        //{
        //    return Roles.Contains(role);
        //}

        //public bool hasAnyGivenRole(Role[] roles)
        //{
        //    return Roles.Any(r => roles.Contains(r));
        //}

        //public bool hasOnlyRole(Role role)
        //{
        //    return Roles.Length == 1 && Roles[0] == role;
        //}

        //public bool hasAreaDivision(string areaDivision)
        //{
        //    return AreaDivisions.Any(a => a.GetStringValue().Equals(areaDivision, StringComparison.OrdinalIgnoreCase));
        //}
    }
}
