using UnityEngine;
using UnityEngine.Serialization;

/**
 * This script assumes that <see>
 *     <cref>SyncObjects</cref>
 * </see>
 * SyncObjects.cs is already used to sync all objects *
 */
public class SyncObject : MonoBehaviour
{
    private readonly Rdf _rdf = Rdf.Instance();
    public string type;
    [FormerlySerializedAs("Repository")] public string repository = "http://localhost:8090/rdf4j/repositories/dummy_knowledge";
    public string server = "http://localhost:7200";
    public string repo = "dummy_knowledge";
    private Vector3 _lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        _lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != _lastPosition)
        {
            _rdf.UpdateRdfRepresentation(gameObject, type);
        }
    }
}
