using UnityEngine;
using System.Collections;

public class BackgroundPlaneController : MonoBehaviour
{

    [Tooltip("The the Child of the ARCamer's -> StereoCameraLeft's -> Background plane Object here")]
    [SerializeField] private GameObject m_BackgroundPlane;

    // Use this for initialization
    IEnumerator SetBGPlane()
    {
        yield return new WaitForSeconds(1);

        m_BackgroundPlane = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("BackgroundPlane").gameObject;

        m_BackgroundPlane.transform.localPosition = new Vector3(
            m_BackgroundPlane.transform.position.x,
            m_BackgroundPlane.transform.position.y,
            // This is my custom value /// You may need to find your own by checking the same first in Editor  
            // THen write the same here .. // OR you can set the range variation etc etc 
            1178

        );
    }

    // Update is called once per frame
    private void Start()
    {
        StartCoroutine(SetBGPlane());
    }
}