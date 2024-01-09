using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using VDS.RDF;
using VDS.RDF.Ontology;
using VDS.RDF.Parsing;
using VDS.RDF.Storage;
using Object = UnityEngine.Object;

public class Rdf
{
    IGraph _g;
    INode _rootNode;
    private static Rdf _rdf;
    
    private Rdf()
    {
        
    }

    public static Rdf Instance()
    {
        if(_rdf==null)
        {
            _rdf = new Rdf();
        }
        return _rdf;
    }

    public void SetGraph(ref IGraph graph, string rootUri="")
    {
        _g = graph;
    }

    public void CreateRootNode(string rootUri)
    {
        _rootNode = _g.CreateUriNode(new Uri(rootUri));
    }

    public void UpdateAllSceneObjects(ref IGraph graph, string rootUri, List<string> objectsOfInterest)
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in allObjects)
        {
            string type = GetType(gameObject.name, objectsOfInterest);
            if (type != null)
            {
                UpdateRdfRepresentation(gameObject, type);
            }
        }
    }
    private static string GetType(string name, List<string> objectsOfInterest)
    {
        foreach (string objectOfInterest in objectsOfInterest)
        {
            if (name.ToLower().Contains(objectOfInterest.ToLower()))
            {
                return (objectOfInterest!="Rack")?objectOfInterest:"Shelf";
            }
        }
        return null;
    }

    public void UpdateRdfRepresentation(GameObject obj, string type)
    {
        // INode _contains = graph.CreateUriNode(new Uri("http://www.dfki.de/unity-ns#contains"));
        INode contains = _g.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject));
        Uri stringNode = new Uri(XmlSpecsHelper.XmlSchemaDataTypeString);
        // INode s = _g.CreateBlankNode();
        INode s = _g.CreateUriNode(new Uri("https://www.dfki.de/unity-ns#GameObject" + obj.GetInstanceID()));

        INode rdftype = _g.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        INode typeNode = _g.CreateLiteralNode(type, stringNode);

        
        INode label = _g.CreateUriNode(new Uri(OntologyHelper.PropertyLabel));
        INode name = _g.CreateLiteralNode(obj.name,stringNode);
        if (obj.name == "QuadCopter")
        {
            Debug.Log("Sending a QuadCopter");
        }
        
        INode positionNode = _g.CreateUriNode(new Uri("https://www.dfki.de/unity-ns#position"));
        INode position = _g.CreateLiteralNode(obj.transform.position.ToString(), stringNode);
        
        INode rotationNode = _g.CreateUriNode(new Uri("https://www.dfki.de/unity-ns#rotation"));
        INode rotation = _g.CreateLiteralNode(obj.transform.rotation.ToString(), stringNode);

        INode forwardNode = _g.CreateUriNode(new Uri("https://www.dfki.de/unity-ns#forward"));
        INode forward = _g.CreateLiteralNode(obj.transform.forward.ToString(), stringNode);

        _g.Assert(new Triple(_rootNode, contains, s));
        _g.Assert(new Triple(s, rdftype, typeNode));
        _g.Assert(new Triple(s, label, name));
        Triple positionTriple = new Triple(s, positionNode, position);
        // Triple newTriple = _g.GetTripleNode(positionTriple).Triple;
        // _g.Retract(newTriple);        
        
        _g.Assert(positionTriple);
        _g.Assert(new Triple(s, rotationNode, rotation));
        _g.Assert(new Triple(s, forwardNode, forward));
    }
}

public class SyncObjects : MonoBehaviour
{
    [FormerlySerializedAs("Repository")] public string repository = "http://localhost:8090/rdf4j/repositories/dummy_knowledge";
    [FormerlySerializedAs("rdf_server")] public string rdfServer = "http://localhost:7200";
    [FormerlySerializedAs("repo_name")] public string repoName = "dummy_knowledge";
    [FormerlySerializedAs("graph_name_uri")] public string graphNameUri = "https://www.dfki.de/unity-ns#GameObjects";
    [FormerlySerializedAs("_objectsOfInterest")] public List<string> objectsOfInterest = new List<string>{ "Box", "Shelf", "Rack", "Worker", "QuadCopter"};

    private Rdf Rdf { get; } = Rdf.Instance();

    public void SyncAll()
    {
        SesameHttpProtocolVersion6Connector graphDB =
            new SesameHttpProtocolVersion6Connector(rdfServer, repoName);
        IGraph existingGraph = new Graph();
        INode s = existingGraph.CreateUriNode(new Uri(graphNameUri));
        INode p = existingGraph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        INode o = existingGraph.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject));
        Triple t = new Triple(s, p, o);
        graphDB.UpdateGraphAsync(graphNameUri, new List<Triple>(){t}, null,new CancellationToken(false));
        graphDB.LoadGraph(existingGraph, graphNameUri);
        graphDB.DeleteGraph(graphNameUri);
        Rdf.SetGraph(ref existingGraph);
        Rdf.CreateRootNode(graphNameUri);
        Rdf.UpdateAllSceneObjects(ref existingGraph, graphNameUri, objectsOfInterest);
        graphDB.SaveGraph(existingGraph);
    }
}
