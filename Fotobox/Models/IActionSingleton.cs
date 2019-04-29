namespace Fotobox.Models
{
  public interface IActionSingleton
  {
    bool IsLocked { get; set; }

    string Picture { get; set; }
  }
}
