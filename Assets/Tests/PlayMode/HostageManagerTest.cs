using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HostageManagerTest
{
    private GameObject hostageManagerObject;
    private HostageManager hostageManager;
    private GameObject gameManagerObject;
    private GameManager2D gameManager;
    private GameObject mapGeneratorObject;
    private MapGenerator mapGenerator;

    [SetUp]
    public void SetUp()
    {
        // HostageManager létrehozása
        hostageManagerObject = new GameObject("TestHostageManager");
        hostageManager = hostageManagerObject.AddComponent<HostageManager>();
        
        // GameManager2D mock létrehozása
        // Az Awake() automatikusan beállítja az Instance-t
        gameManagerObject = new GameObject("TestGameManager");
        gameManager = gameManagerObject.AddComponent<GameManager2D>();
        
        // MapGenerator mock létrehozása
        mapGeneratorObject = new GameObject("TestMapGenerator");
        mapGenerator = mapGeneratorObject.AddComponent<MapGenerator>();
        gameManager.mapGenerator = mapGenerator;
    }

    [TearDown]
    public void TearDown()
    {
        if (hostageManagerObject != null) Object.DestroyImmediate(hostageManagerObject);
        if (gameManagerObject != null) Object.DestroyImmediate(gameManagerObject);
        if (mapGeneratorObject != null) Object.DestroyImmediate(mapGeneratorObject);
    }

    [UnityTest]
    public IEnumerator HostageManager_InitializeHostageCount_SetsTotalHostages()
    {
        // Várunk egy frame-et, hogy az Awake() metódusok meghívódjanak
        yield return null;
        
        int hostageCount = 5;
        
        hostageManager.InitializeHostageCount(hostageCount);
        
        yield return null;
        
        Assert.AreEqual(hostageCount, hostageManager.GetTotalHostages(), "A túszok száma beállítva");
        Assert.AreEqual(0, hostageManager.GetCollectedHostages(), "A mentett túszok száma 0");
    }

    [UnityTest]
    public IEnumerator HostageManager_CollectHostage_IncrementsCollectedCount()
    {
        hostageManager.InitializeHostageCount(3);
        
        GameObject hostageObject = new GameObject("TestHostage");
        Hostage2D hostage = hostageObject.AddComponent<Hostage2D>();
        
        hostageManager.CollectHostage(hostage);
        
        yield return null;
        
        Assert.AreEqual(1, hostageManager.GetCollectedHostages(), "A mentett túszok száma növekedett");
        Assert.IsFalse(hostageManager.AreAllHostagesCollected(), "Még nincs minden túsz megmentve");
        
        Object.DestroyImmediate(hostageObject);
    }

    [UnityTest]
    public IEnumerator HostageManager_CollectAllHostages_TriggersBossSpawn()
    {
        hostageManager.InitializeHostageCount(2);
        mapGenerator.spawnBossInLastRoom = true;
        
        // Mock túszok létrehozása és mentése
        GameObject hostage1 = new GameObject("Hostage1");
        Hostage2D hostage1Component = hostage1.AddComponent<Hostage2D>();
        
        GameObject hostage2 = new GameObject("Hostage2");
        Hostage2D hostage2Component = hostage2.AddComponent<Hostage2D>();
        
        hostageManager.CollectHostage(hostage1Component);
        yield return null;
        
        Assert.IsFalse(hostageManager.AreAllHostagesCollected(), "Még nincs minden túsz megmentve");
        
        hostageManager.CollectHostage(hostage2Component);
        yield return null;
        
        Assert.IsTrue(hostageManager.AreAllHostagesCollected(), "Minden túsz meg van mentve");
        
        Object.DestroyImmediate(hostage1);
        Object.DestroyImmediate(hostage2);
    }

    [UnityTest]
    public IEnumerator HostageManager_AreAllHostagesCollected_ReturnsFalse_WhenNotAllCollected()
    {
        hostageManager.InitializeHostageCount(5);
        
        GameObject hostage = new GameObject("TestHostage");
        Hostage2D hostageComponent = hostage.AddComponent<Hostage2D>();
        
        hostageManager.CollectHostage(hostageComponent);
        
        yield return null;
        
        Assert.IsFalse(hostageManager.AreAllHostagesCollected(), "Nem minden túsz van megmentve");
        
        Object.DestroyImmediate(hostage);
    }

    [UnityTest]
    public IEnumerator HostageManager_AreAllHostagesCollected_ReturnsTrue_WhenAllCollected()
    {
        hostageManager.InitializeHostageCount(2);
        
        GameObject hostage1 = new GameObject("Hostage1");
        Hostage2D hostage1Component = hostage1.AddComponent<Hostage2D>();
        
        GameObject hostage2 = new GameObject("Hostage2");
        Hostage2D hostage2Component = hostage2.AddComponent<Hostage2D>();
        
        hostageManager.CollectHostage(hostage1Component);
        hostageManager.CollectHostage(hostage2Component);
        
        yield return null;
        
        Assert.IsTrue(hostageManager.AreAllHostagesCollected(), "Minden túsz meg van mentve");
        
        Object.DestroyImmediate(hostage1);
        Object.DestroyImmediate(hostage2);
    }
}

