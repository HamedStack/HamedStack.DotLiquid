﻿using System.Reflection;
using System.Text.RegularExpressions;
using DotLiquid;

namespace HamedStack.DotLiquid
{
    public static class DotLiquidUtility
    {
        internal static readonly string Indicator = "%@%@@%@%";

        // Func<string, IEnumerable<string>, bool> => Tag, Filters
        public static RenderResult RenderWithValidation(string templateText, object data, Func<string, IEnumerable<string>, bool> ignoreValidationCondition = null)
        {
            Template.RegisterFilter(typeof(VariableFilter));
            var tmpl = Template.Parse(templateText);

            if (tmpl.Errors.Any())
            {
                return new RenderResult
                {
                    Template = tmpl,
                    Result = null,
                    Errors = tmpl.Errors.Select(x => x.Message)
                };
            }

            var vars = tmpl.GetAllNodes().Where(x => x is Variable);

            foreach (Variable item in vars)
            {
                var filters = item.Filters.Select(x => x.Name);

                var ignore = ignoreValidationCondition != null && (ignoreValidationCondition?.Invoke(item.Name, filters) == true);

                if (!filters.Contains("ignore_safe_var") || !ignore)
                {
                    templateText = templateText.Replace(item.Name, $"{item.Name} | safe_var:'{item.Name}' ");
                }
            }
            var template2 = Template.Parse(templateText);
            var result = template2.RenderAnonymousObject(data);
            var matches = Regex.Matches(result, $"{Indicator}(.+){Indicator}", RegexOptions.Compiled)
                .Cast<Match>()
                .Select(x => x.Value.Replace(Indicator, "")).Distinct();

            if (matches.Any())
            {
                var exceptions = new List<string>();
                foreach (var match in matches)
                {
                    exceptions.Add($"'{match}' variable is null or does not exist.");
                }
                return new RenderResult
                {
                    Template = tmpl,
                    Result = null,
                    Errors = exceptions
                };
            }

            return new RenderResult
            {
                Template = tmpl,
                Result = result,
                Errors = null
            };

        }
        public static void RegisterSafeTypes(params Type[] types)
        {
            if (types == null || types.Length <= 0) return;

            foreach (var type in types)
            {
                var props = type.GetTypeInfo().GetProperties().Select(x => x.Name).ToArray();
                Template.RegisterSafeType(type, props);
            }
        }
        public static void RegisterSafeTypes(bool withReferencedAssemblies = false)
        {
            var types = new List<Type>();
            types.AddRange(Assembly.GetEntryAssembly()?.GetTypes() ?? throw new InvalidOperationException());

            if (withReferencedAssemblies)
            {
                var refAsm = Assembly.GetEntryAssembly()?.GetReferencedAssemblies();
                if (refAsm != null)
                {
                    foreach (var referencedAssembly in refAsm)
                    {
                        var loadedAssembly = Assembly.Load(referencedAssembly);
                        types.AddRange(loadedAssembly.GetTypes());
                    }
                }
            }

            foreach (var type in types)
            {
                var props = type.GetTypeInfo().GetProperties().Select(x => x.Name).ToArray();
                Template.RegisterSafeType(type, props);
            }
        }
        public static void RegisterSafeTypes(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var props = type.GetTypeInfo().GetProperties().Select(x => x.Name).ToArray();
                Template.RegisterSafeType(type, props);
            }
        }
        public static void RegisterSafeTypes(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var props = type.GetTypeInfo().GetProperties().Select(x => x.Name).ToArray();
                    Template.RegisterSafeType(type, props);
                }
            }
        }
    }
}
