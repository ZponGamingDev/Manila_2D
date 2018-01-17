using System.Collections;

namespace ManilaSceneBase
{
    public abstract class SceneBase
    {
        public string name;
        public int index;

        public abstract IEnumerator LoadScene();
        public abstract IEnumerator Active();
        public abstract IEnumerator UnloadScene();
    }
}
