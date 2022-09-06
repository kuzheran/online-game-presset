using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    private GameObject[] _wayPoints;
    private Transform _enemyPosition;
    private NavMeshAgent _enemyAgent;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _wayPoints = new GameObject[14];
        for (int i = 0; i < 14; i++)
        {
            _wayPoints[i] = GameObject.Find("Point (" + i.ToString() + ")");
        }
        _enemyAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        _enemyPosition = gameObject.GetComponent<Transform>();
        gameObject.name = "ENEMY";

        if (gameObject.GetComponent<PhotonView>().IsMine)
        {
            StartCoroutine(AI());
        }
        else
        {
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<EnemyScript>().enabled = false;
        }
    }
    private IEnumerator AI()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            int key = Random.Range(0, 12);
            _enemyAgent.SetDestination(_wayPoints[key].transform.position);
            while (_enemyPosition.position.x != _wayPoints[key].transform.position.x)
            {
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < 5; i++)
            {
                PhotonNetwork.Instantiate("Бобик", _enemyPosition.position, Quaternion.identity);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.name == PhotonNetwork.NickName)
        {
            _gameManager.DeathAction();
        }
    }
}
