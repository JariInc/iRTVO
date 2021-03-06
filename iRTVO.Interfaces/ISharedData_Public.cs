﻿using System;
using System.Collections.Generic;
namespace iRTVO.Interfaces
{
    public interface ISharedData
    {        
        ICameraInfo Camera { get;  }
        IList<IDriverInfo> Drivers { get;  }
        IList<ISessionEvent> Events { get;  }
        ISessions Sessions { get;  }
        ITrackInfo Track { get;  }
        ISettings Settings { get; }

        object SharedDataLock { get; }

        void SwitchCamera(int driver, int camera);
    }
}
