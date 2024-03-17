using DotLiquid;

namespace HamedStack.DotLiquid
{
    public class RenderResult
    {
        public Template Template { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string Result { get; set; }
    }
}