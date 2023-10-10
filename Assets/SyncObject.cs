using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SyncObject : MonoBehaviour
{
    private readonly RDF _rdf;
    public string type;
    public string Repository = "http://localhost:8090/rdf4j/repositories/dummy_knowledge";
    public string server = "http://localhost:7200";
    public string repo = "dummy_knowledge";
    private Vector3 lastPosition;
    // Start is called before the first frame update
    public SyncObject()
    {
        _rdf = new RDF(Repository);
    }
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.position.Equals(lastPosition))
        {
            string updateRDFString = _rdf.GetRDFRepresentation(gameObject, type);
            StartCoroutine(_rdf.Upload(updateRDFString));
        }
    }
}
