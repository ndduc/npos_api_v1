using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_Api.Model.EnumData
{
    public static class ResponseEnum
    {
        public enum AddUpdateEnum {
            SUCESS = 0,
            FAIL = 1,
            NOT_PROVIDED = 2,
        };
    }
}
