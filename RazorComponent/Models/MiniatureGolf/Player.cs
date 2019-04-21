using System;

namespace RazorComponent.Models.MiniatureGolf
{
    public class Player
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
    }
}
