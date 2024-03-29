﻿using DotLiquid;

namespace HamedStack.DotLiquid
{
    public static class Extensions
    {        
        public static string RenderAnonymousObject(this Template template, object obj, bool includeBaseClassProperties = false, IFormatProvider formatProvider = null)
        {
            return template.Render(Hash.FromAnonymousObject(obj, includeBaseClassProperties), formatProvider);
        }
        public static string RenderObject(this Template template, object obj, bool includeBaseClassProperties = false, IFormatProvider formatProvider = null)
        {
            return template.Render(Hash.FromAnonymousObject(new { RootObject = obj }, includeBaseClassProperties), formatProvider);
        }
        public static IEnumerable<object> GetAllNodes(this Template template)
        {
            return ProcessNodes(template.Root.NodeList);

            static IEnumerable<object> ProcessNodes(IEnumerable<object> nodes)
            {
                var result = new List<object>();

                if (nodes != null && nodes.Any())
                {
                    foreach (var node in nodes)
                    {
                        if (node is Tag tag)
                        {
                            var newResult = ProcessNodes(tag.NodeList);
                            result.AddRange(newResult);
                        }
                        result.Add(node);
                    }
                }

                return result;
            }
        }
    }
}
