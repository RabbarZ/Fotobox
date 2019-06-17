using System;
using System.Collections.Generic;

namespace Fotobox.Models
{
    public interface IActionSingleton
    {
        bool IsLocked { get; set; }

        string PicturePath { get; set; }

        List<TimeSpan> TimeOccured { get; set; }
    }
}
