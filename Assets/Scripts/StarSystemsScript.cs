using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystemsScript : MonoBehaviour
{
    public TextAsset jsonFile;
    public int cameraMode; // 0 = Galaxy view, 1 = System view, 2 = Celestial object view
    public GameObject StarSystemPrefab;
    public GameObject mainCamera;
    public GameObject selectedSystem;
    public GameObject selectedObject;
    public GameObject UIContainer;

    public int layerMask;
    // Start is called before the first frame update
    void Start()
    {
        StarSystems jsonStarSystems = JsonUtility.FromJson<StarSystems>(jsonFile.text);
        foreach (StarSystem starSystem in jsonStarSystems.starSystems)
        {
            GameObject StarSystemGO = Instantiate(StarSystemPrefab, new Vector3(starSystem.posX * 7, starSystem.posZ * 7, starSystem.posY * 7), Quaternion.identity);
            StarSystemGO.name = starSystem.name;
            StarSystemGO.transform.localScale *= 20;

            StarSystemGO.AddComponent<SystemsInfosScript>();
            SystemsInfosScript starSystemInfos = StarSystemGO.GetComponent<SystemsInfosScript>();
            starSystemInfos.description = starSystem.description;
            starSystemInfos.type = starSystem.type;
            starSystemInfos.systemName = starSystem.name;
            starSystemInfos.status = starSystem.status;
            starSystemInfos.lastTimeModified = starSystem.time_modified;
            starSystemInfos.size = starSystem.aggregated_size;
            starSystemInfos.population = starSystem.aggregated_population;
            starSystemInfos.economy = starSystem.aggregated_economy;
            starSystemInfos.danger = starSystem.aggregated_danger;


            foreach(Affiliation affiliation in starSystem.affiliation)
            {
                starSystemInfos.affiliationID = affiliation.id;
                starSystemInfos.affiliationName = affiliation.name;
            }

            starSystemInfos.json = System.IO.File.ReadAllText(Application.dataPath + "/Jsons/Systems/" + starSystem.name + ".json");
            if(starSystem.id == 320)starSystemInfos.json = System.IO.File.ReadAllText(Application.dataPath + "/Jsons/Systems/Nul1.json"); //a file can't be named nul so... 

            

        }
        cameraMode = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(cameraMode == 0)
        {
            if(Input.GetMouseButtonUp(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, 500))
                {
                    if(hit.transform)
                    {
                        SelectSystem(hit.transform.gameObject);
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.Return))
            {
                LoadAndEnterSystem();
            }
        }
        else if(cameraMode == 1)
        {
            if(Input.GetMouseButtonUp(1))
            {
                layerMask = 1 << LayerMask.NameToLayer("CO");
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, 100, layerMask))
                {
                    if(hit.transform)
                    {
                        selectedObject = hit.transform.gameObject;
                        mainCamera.GetComponent<CameraScript>().MoveToCO(selectedObject);
                        //cameraMode = 2;
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                UnloadAndExitSystem();
            }

        }
    }

    public void SelectSystem(GameObject gameObject)
    {
        selectedSystem = gameObject;
        mainCamera.GetComponent<CameraScript>().SelectSystem(selectedSystem);
        DiscScript discScript = UIContainer.GetComponent<DiscScript>();
        discScript.selectedObject = selectedSystem;
        discScript.LoadDisc();
        
        print(gameObject.GetComponent<SystemsInfosScript>().description);
    }
    public void LoadAndEnterSystem()
    {
        UIContainer.GetComponent<DiscScript>().UnloadDisc();
        this.GetComponent<StarSystemGeneration>().LoadSystem(selectedSystem);
        mainCamera.GetComponent<CameraScript>().EnterSystem(selectedSystem);
        UIContainer.GetComponent<SystemNameScript>().ChangeName(selectedSystem.name);
        cameraMode = 1;
    }

    public void UnloadAndExitSystem()
    {
        this.GetComponent<StarSystemGeneration>().UnloadSystem(selectedSystem);
        cameraMode = 0;
        mainCamera.GetComponent<CameraScript>().ExitSystem(selectedSystem);
        UIContainer.GetComponent<SystemNameScript>().ChangeName("");
    }

}

[System.Serializable]
public class StarSystem
{
    public int id;
    public float posX;
    public float posY;
    public float posZ;
    public string name;
    public string description;
    public string type;
    public string status;
    public string time_modified;
    public string aggregated_size;
    public string aggregated_population;
    public string aggregated_economy;
    public string aggregated_danger;
    public Affiliation[] affiliation;

    
}

[System.Serializable]
public class StarSystems
{
    public StarSystem[] starSystems;
}

[System.Serializable]
public class Affiliation
{
    public string id;
    public string name;
}


