using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;


        ConnectBaseScriptableObject();

        Register<SkillManager, SkillManagerConfigSO>(config => new SkillManager(config));
        Register<SoundManager, SoundManagerConfigSO>(config => new SoundManager(config));
        Register<DecalPool, DecalPoolConfigSO>(config => new DecalPool(config));
        Register<SceneLoadManager, SceneLoadManagerConfigSO>(config => new SceneLoadManager(config));
        Register<CharacterSelectionManager, CharacterSelectionManagerConfigSO>(config => new CharacterSelectionManager(config));
        GetControllerAll();
        InitAll();
        ActiveOffAll();
       
    }

    void OnDisable()
    {
        foreach (var manager in managerMap.Values)
        {
            manager.Destory();
        }
    }



    void ConnectBaseScriptableObject()
    {
        dicBaseScriptableObject.Clear();

        foreach (var so in baseScriptableObjects)
        {
            if (so == null) continue;
            dicBaseScriptableObject[so.GetType()] = so;
        }
    }


    private void Register<TManager, TConfig>(Func<TConfig, TManager> factory) where TManager : baseManager where TConfig : BaseScriptableObject
    {
    
        TConfig config = (TConfig)dicBaseScriptableObject[typeof(TConfig)];
        TManager manager = factory(config);
        RegisterMap(manager);
    }

    private void RegisterMap<T1>(T1 manager) where T1 : baseManager
    {
        managerMap[typeof(T1)] = manager;
    }

    private void InitAll()
    {
        foreach (var manager in managerMap.Values)
        {
            manager.Init();
        }
    }

    private void ActiveOffAll()
    {
        foreach (var manager in managerMap.Values)
        {
            manager.ActiveOff();
        }
    }
    private void GetControllerAll()
    {
        foreach (var manager in managerMap.Values)
        {
            manager.GetController(this);
        }
    }
    private void UpdateAll()
    {
        foreach (var manager in managerMap.Values)
        {
            manager.Update();
        }
    }


    public T GetManager<T>() where T : baseManager
    {
        return (T)managerMap[typeof(T)];

    }

    void Update()
    {
        UpdateAll();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (managerMap.TryGetValue(
    typeof(SceneLoadManager),
    out baseManager manager))
        {
            SceneLoadManager sceneLoadManager =
                manager as SceneLoadManager;

            sceneLoadManager?.OnSceneLoaded();
        }
    }


    private Dictionary<Type, baseManager> managerMap = new Dictionary<Type, baseManager>();
    private Dictionary<Type, BaseScriptableObject> dicBaseScriptableObject = new Dictionary<Type, BaseScriptableObject>();

    public Transform playerTransform;
    public static GameController instance;

    [SerializeField]
    private List<BaseScriptableObject> baseScriptableObjects = new List<BaseScriptableObject>();
}