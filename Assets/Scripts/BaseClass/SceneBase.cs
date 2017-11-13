using System.Collections;
using System.Collections.Generic;

namespace ManilaSceneBase
{
    public abstract class SceneBase
    {
        public string name;
        public int index;

        public abstract void Loading();
        public abstract void Initializing();
        public abstract void Active();
        public abstract void Inactive();
        public abstract void ChangeScene();
    }
}
