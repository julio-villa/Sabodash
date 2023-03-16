using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField] public Transform Lobby;
    [SerializeField] private Camera cam;

    private List<GameObject> sections = new List<GameObject>();

    public Vector3 latestSectionEndPos;

    private const float GENERATION_DIST = 20f;

    public List<Transform> renderedSections = new List<Transform>();

    private bool firstSpawn = true;

    private void Awake()
    {
        for (int i = 1; i < 36; i++)
        {
            string path = "Level Sections/Active Sections/Section " + i;
            GameObject loaded_section = Resources.Load(path) as GameObject;
            sections.Add(loaded_section);
        }    
    }

    // Start is called before the first frame update
    private void Start()
    {
        latestSectionEndPos = Lobby.Find("SectionEnd").position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(cam.GetComponent<Transform>().position, latestSectionEndPos) < GENERATION_DIST)
        {

            // Spawn new section
            SpawnLevelSection();

            // Delete old sections
            if (renderedSections.Count > 3)
            {
                Transform deleting = renderedSections[0];
                renderedSections.RemoveAt(0);
                Destroy(deleting.GameObject());
            }
        }
    }

    public void SpawnLevelSection()
    {
        Transform latestSectionTransform = SpawnLevelSection(latestSectionEndPos).transform;
        renderedSections.Add(latestSectionTransform);

        latestSectionEndPos = latestSectionTransform.Find("SectionEnd").position;

        // Update game speed
        if (!firstSpawn)
        {
            GameState.gameSpeed *= 1.05f;
            foreach (Player p in GameState.alivePlayers)
            {
                p.updateGravity();
            }
        } else
        {
            firstSpawn = false;
        }
        
    }
        

    private GameObject SpawnLevelSection(Vector3 spawnPosition)
    {

        GameObject spawning_section = sections[Random.Range(0, sections.Count)];

        GameObject section = Instantiate(spawning_section, spawnPosition, Quaternion.identity);
        return section;
    }

}
