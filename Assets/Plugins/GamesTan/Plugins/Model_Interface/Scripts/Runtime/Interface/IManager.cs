using System;
using System.Text;

namespace RealDream {

    public interface IManager {
        void DoAwake();
        void DoStart();
        void DoUpdate(float deltaTime);
        void DoFixedUpdate(float deltaTime);
        void DoDestroy();

    }
}