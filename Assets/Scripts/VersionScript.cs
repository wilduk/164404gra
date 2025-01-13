using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionScript : MonoBehaviour
{

    [SerializeField]
    private int majorVersion = 1;
    [SerializeField]
    private int minorVersion = 0;
    [SerializeField]
    private int patchVersion = 0;
    [SerializeField]
    private string versionText;
    // Start is called before the first frame update
    void Start()
    {
        string end = "";
        #if UNITY_WEBGL
            end = "web";
        #endif
        gameObject.GetComponent<TextMeshProUGUI>().text = "Version: "+majorVersion+"."+minorVersion+"."+patchVersion+versionText+end;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
