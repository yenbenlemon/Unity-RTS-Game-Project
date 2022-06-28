using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Switch scenes on key press
        if (Input.anyKey)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
