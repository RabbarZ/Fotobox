using System;

namespace Fotobox.Models
{
  public class ActionSingleton : IActionSingleton
  {
    public bool IsLocked { get; set; }

    //private static ActionSingleton instance;

    //public ActionSingleton Instance => instance ?? (instance = new ActionSingleton());

    //private ActionSingleton()
    //{
    //}
  }
}