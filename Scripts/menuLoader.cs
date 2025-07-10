using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuLoader : MonoBehaviour
{
    public void loadMain()
    {
        SceneManager.LoadScene("mainMenu", LoadSceneMode.Single);

    }
}
