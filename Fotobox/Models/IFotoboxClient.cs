using System.Threading.Tasks;

namespace Fotobox.Models
{
    public interface IFotoboxClient
    {
        Task ShowText(string text);

        Task ReloadPicture(string path);

        Task Reset(string text);
    }
}
