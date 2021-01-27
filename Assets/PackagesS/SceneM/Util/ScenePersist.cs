namespace SceneM
{
    public class ScenePersist : Singleton<ScenePersist>
    {
        public override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            
        }
    } 
}
