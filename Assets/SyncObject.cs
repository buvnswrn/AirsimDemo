using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script assumes that <see>
 *     <cref>SyncObjects</cref>
 * </see>
 * SyncObjects.cs is already used to sync all objects *
 */
public class SyncObject : MonoBehaviour
{
    private readonly RDF _rdf = RDF.Instance();
    public string type;
    public string Repository = "http://localhost:8090/rdf4j/repositories/dummy_knowledge";
    public string server = "http://localhost:7200";
    public string repo = "dummy_knowledge";
    private Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != lastPosition)
        {
            _rdf.UpdateRDFRepresentation(gameObject, type);
        }
    }
}
