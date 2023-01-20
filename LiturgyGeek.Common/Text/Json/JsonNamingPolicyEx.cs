using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Text.Json
{
    public abstract class JsonNamingPolicyEx : JsonNamingPolicy
    {
        public static JsonNamingPolicyEx CamelCaseEx { get; } = new CamelCaseNamingPolicy();

        private class CamelCaseNamingPolicy : JsonNamingPolicyEx
        {
            public override string ConvertName(string name)
            {
                return name.Length > 0 && name[0] == '_'
                    ? '_' + CamelCase.ConvertName(name.Substring(1))
                    : CamelCase.ConvertName(name);
            }
        }
    }
}
