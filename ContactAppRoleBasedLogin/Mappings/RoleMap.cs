﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppRoleBasedLogin.Models;
using FluentNHibernate.Mapping;

namespace ContactAppRoleBasedLogin.Mappings
{
    public class RoleMap:ClassMap<Role>
    {
        public RoleMap()
        {
            Table("Roles");
            Id(r => r.Id).GeneratedBy.GuidComb();
            Map(r => r.RoleName);
            References(r => r.User).Column("UserId").Unique().Cascade.None();
        }
    }
}