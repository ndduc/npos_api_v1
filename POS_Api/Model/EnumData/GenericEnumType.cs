﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model.EnumData
{
    public static class GenericEnumType
    {
        public enum UserType { ADMIN , DEV , OWNER , MANAGER , EMPLOYEE };
        public enum UserLocationType { CREATED, EMPLOYED};
    }
}
