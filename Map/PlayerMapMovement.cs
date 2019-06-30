using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMapMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D body;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        var movement = new Vector2(moveH, moveV);
        body.velocity = movement * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "MapEnemy")
        {
            PartyManager.Instance.Encounter = other.GetComponent<EnemyMap>().encounter;
            Destroy(other.gameObject);

            SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
            var currentScene = SceneManager.GetActiveScene();

            // TODO: Do it better nigga!
            foreach (var obj in currentScene.GetRootGameObjects())
            {
                obj.SetActive(false);
            }
        }
        else if (other.tag == "Portal")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (other.tag == "MapEnd")
        {
            // PartyManager gibe xp plox
            SceneManager.LoadScene("WorldMap");
        }
        else if (other.tag == "Chest")
        {
            var chest = other.GetComponent<Chest>();
            InventoryController.AddItem(chest.Equipment);

            Destroy(other.gameObject);
        }
    }
}
