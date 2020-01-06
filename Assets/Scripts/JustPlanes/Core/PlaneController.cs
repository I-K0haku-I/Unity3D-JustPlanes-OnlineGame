using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustPlanes.Core
{
    public class PlaneController : IPlaneController
    {
        public void Tick()
        {
            throw new NotImplementedException();
        }
    }

    public interface IPlaneController
    {

        void Tick();

    }
}
