using System;

namespace Fotobox.Models
{
  public class ActionSingleton : IActionSingleton
  {
    public bool IsLocked { get; set; }

    public string Picture { get; set; }
  }
}