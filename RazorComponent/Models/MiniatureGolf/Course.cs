using System.Collections.Generic;

namespace RazorComponent.Models.MiniatureGolf
{
    public class Course
    {
        public int Number { get; set; }
        public int Par { get; set; }

        public Dictionary<string, int?> PlayerHits { get; set; } = new Dictionary<string, int?>();
    }
}
