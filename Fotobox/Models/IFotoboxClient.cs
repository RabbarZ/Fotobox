using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotobox.Models
{
    public interface IFotoboxClient
    {
        Task ChangeCountdown(int number);

        Task ReloadPicture();

        Task Reset(string text);
    }
}
