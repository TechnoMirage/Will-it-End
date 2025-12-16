using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Threading;
using UnityEngine.Rendering;
using Player;

public class HandleRounds : MonoBehaviour
{
    private GameObject _spawners;
    private GameObject _ammoSpawners;
    private Transform _zombiesParent;
    private int _round;
    private TextMeshProUGUI _roundText;
    private TextMeshProUGUI _zombieText;
    private bool _isRoundOver = false;
    private float _maxZombieSpeed = 2f;

    private bool zombiesAreReadyToSpawn = true;

    private float _zombieIncreaseSpeed = 0f;
    private int _zombieIncreaseHealth = 0;

    public GameObject zombieModel;
    public GameObject ammoBoxModel;

    // Start is called before the first frame update
    void Start()
    {
        _round = 1;
        _spawners = GameObject.Find("Spawners");
        _ammoSpawners = GameObject.Find("AmmoBoxSpawners");
        _zombiesParent = GameObject.Find("Zombies").transform;
        _roundText = GameObject.Find("RoundCount").GetComponent<TextMeshProUGUI>();
        _zombieText = GameObject.Find("ZombieCount").GetComponent<TextMeshProUGUI>();

        StartCoroutine(SpawnZombies());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateZombieText();

        if (_zombiesParent.childCount == 0)
        {
            if (!_isRoundOver)
            {
                _isRoundOver = true;
                StartCoroutine(StartNextRoundAfterDelay());              
            }
        }
    }

    void UpdateRound()
    {
        _round++;
        _roundText.text = _round.ToString();

        if (_round % 2 != 0)
        {
            SpawnAmmoBox();
        }
    }

    void UpdateZombieText() 
    {
        _zombieText.text = _zombiesParent.childCount.ToString();
    }

    IEnumerator StartNextRoundAfterDelay()
    {
        UpdateRound();

        float timer = 0;
        while (timer < 10f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        IncreaseZombieStats();
        StartCoroutine(SpawnZombies());
    }

    void IncreaseZombieStats()
    {
        if (_zombieIncreaseSpeed < _maxZombieSpeed)
        {
            _zombieIncreaseSpeed += 0.2f;
        }

        _zombieIncreaseHealth += 2;
    }

    private void AsignNewStats(GameObject zombie)
    {
        zombie.GetComponent<ZombieController>().increaseSpeed += _zombieIncreaseSpeed;
        zombie.GetComponent<ZombieHealth>().IncreaseHealth(_zombieIncreaseHealth);   
    }

    private void SpawnAmmoBox()
    {
        foreach (Transform child in _ammoSpawners.transform)
        {
            if (child.name != "Cube")
            {
                Destroy(child.gameObject);
            }
        }

        int spawnIndex = Random.Range(0, _ammoSpawners.transform.childCount);
        Transform spawnPoint = _ammoSpawners.transform.GetChild(spawnIndex);

        GameObject ammoBox = Instantiate(ammoBoxModel, spawnPoint.position, spawnPoint.rotation);
        ammoBox.transform.parent = _ammoSpawners.transform;
    }

    IEnumerator SpawnZombies()
    {
        int zombiesToSpawn = _round * 10;
        int zombiesSpawned = 0;
        zombiesAreReadyToSpawn = true;
        while (zombiesSpawned < zombiesToSpawn)
        {
            if (zombiesAreReadyToSpawn)
            {
                int spawnIndex = Random.Range(0, _spawners.transform.childCount);
                Transform spawnPoint = _spawners.transform.GetChild(spawnIndex);
                if (spawnPoint != null)
                {
                    float x = Random.Range(-5, 5);
                    float z = Random.Range(-5, 5);

                    GameObject zombie = Instantiate(zombieModel, spawnPoint.position + new Vector3(x, 0.5f, z), Quaternion.identity);
                    AsignNewStats(zombie);
                    zombie.transform.parent = _zombiesParent;
                    zombiesAreReadyToSpawn = false;
                    zombiesSpawned++;
                }
                StartCoroutine(WaitForNextZombie());
            }
            yield return null;
        }
        _isRoundOver = false;
    }

    public bool checkIfMachineShouldSpawnPerk()
    {
        if (_round % 5 == 0)
        {
            return true;
        }

        return false;
    }

    public int getCurrentRound()
    {
        return _round;
    }
    
    IEnumerator WaitForNextZombie()
    {
        float timer = 0;
        while (timer < 0.12)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        zombiesAreReadyToSpawn = true;
    }
}
