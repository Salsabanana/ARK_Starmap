using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHandler : MonoBehaviour
{
    public GameObject scriptHandler;
    public GameObject UIContainer;
    AudioManagerScript audioManager;
    //disc
    private Vector2 pos1 = new Vector2(153, 23.4f);
    private Vector2 pos2= new Vector2(163, 0.2f);
    private Vector2 pos3= new Vector2(158, -24.5f);
    public GameObject inspectButton;
    public GameObject informationsButton;

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterSystem()
    {
        audioManager.play("ClickDisc");
        StartCoroutine(moveButton(inspectButton, pos2, true));
        StartCoroutine(moveButton(informationsButton, pos3, false));
    }

    public void LoadInfobox()
    {
        audioManager.play("ClickDisc");
        StartCoroutine(moveButton(inspectButton, pos1, false));
        StartCoroutine(moveButton(informationsButton, pos2, true));
    }

    public void setInitialPos()
    {
        StartCoroutine(moveButton(inspectButton, pos2, false));
        StartCoroutine(moveButton(informationsButton, pos3, false));
    }

    IEnumerator moveButton(GameObject button, Vector2 pos, bool executeAction)
    {
        Vector2 basePos = button.GetComponent<RectTransform>().anchoredPosition;
        for(float i = 0; i < 1; i += 0.02f)
        {
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(basePos, pos, i);
            yield return null;
        }
        if(executeAction)
        {
            if(button.gameObject.name == "Informations")UIContainer.GetComponent<DiscScript>().LoadInfobox();
            if(button.gameObject.name == "Inspect")scriptHandler.GetComponent<StarSystemsScript>().LoadAndEnterSystem();
        }
    }
}
