using System.Collections;
using System.Collections.Generic;

namespace ManilaSceneBase
{
    public abstract class SceneBase
    {
        public string name;
        public int index;


        public abstract void LoadSceneInitialObj();
        public abstract IEnumerator LoadScene();
        public abstract IEnumerator Active();
        public abstract IEnumerator UnloadScene();
    }
}
