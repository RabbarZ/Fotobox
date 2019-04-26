namespace Fotobox.Models
{
  public interface IActionSingleton
  {
    bool IsLocked { get; set; }

    bool HasUnsavedPicture { get; set; }

    string Picture { get; set; }
  }
}
