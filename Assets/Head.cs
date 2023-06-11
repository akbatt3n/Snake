using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Head : MonoBehaviour {
    
    public bool alive = true;

    public GameObject bodyPrefab;
    public List<GameObject> bodies = new List<GameObject>();

    Vector2 dir = new Vector2(1, 0);
    Vector2 newDir = new Vector2(1, 0);

    Vector3 newPos;
    Vector3 oldPos;

    public Transform food;
    int boundary = 16;

    public TMP_Text scoreText;
    public GameObject scoreObject;

    // starts at 0.4, decreases by 20% each step
    float[] speedValues = {0.4f, 
                            0.32f,
                            0.256f,
                            0.205f,
                            0.164f,
                            0.131f,
                            0.105f,
                            0.084f,
                            0.067f,
                            0.054f,
                            0.043f};
    int[] scoreValues = {3, 6, 12, 24, 48, 72, 100, 150, 200, 250, 300};
    int difficulty = 0;

    void Start() {
        scoreObject.active = false;
        InvokeRepeating("move", speedValues[0], speedValues[0]);
    }

    void move() {

        // move head. only use new direction of 90 deg from current
        if (Vector2.Angle(newDir, dir) == 90) {
            dir = newDir;
        }
        newPos = transform.position;
        transform.Translate(dir.x, dir.y, 0);

        // move all body segments
        foreach(GameObject segment in bodies) {
            oldPos = segment.transform.position;
            segment.transform.position = newPos;
            newPos = oldPos;
        }

        // check for wall collisions
        if (transform.position.y == boundary || transform.position.y == -boundary ||
            transform.position.x == boundary || transform.position.x == -boundary) {

            die();
        }

        // check for body collisions
        foreach(GameObject segment in bodies) {
            if (transform.position == segment.transform.position) {
                die();
            }
        }

        // check for food
        if (transform.position == food.transform.position) {
            if (bodies.Count == 0) {
                bodies.Add(Instantiate(bodyPrefab, this.transform.position, Quaternion.identity));
            }
            else {
                bodies.Add(Instantiate(bodyPrefab, bodies[bodies.Count-1].transform.position, Quaternion.identity));
            }

            food.transform.position = new Vector3(Random.Range(-boundary+1, boundary-1), Random.Range(-boundary+1,  boundary-1));
            if (bodies.Count > scoreValues[difficulty]) {
                difficulty += 1;
                CancelInvoke();
                InvokeRepeating("move", speedValues[difficulty], speedValues[difficulty]);
            }
        }
    }

    void die() {
        alive = false;
        CancelInvoke();
        scoreText.text = "Score: " + bodies.Count;
        scoreObject.active = true;
    }

    public void restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // process key presses
    void Update() {
        if(Input.GetKeyDown("a") || Input.GetKeyDown("left")) {
            newDir = Vector2.left;
        }
        else if (Input.GetKeyDown("d") || Input.GetKeyDown("right")) {
            newDir = Vector2.right;
        }
        else if (Input.GetKeyDown("w") || Input.GetKeyDown("up")) {
            newDir = Vector2.up;
        }
        else if (Input.GetKeyDown("s") || Input.GetKeyDown("down")) {
            newDir = Vector2.down;
        }
    }

}
