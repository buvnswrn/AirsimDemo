using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using VDS.RDF;
using VDS.RDF.Ontology;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Storage;
using VDS.RDF.Writing;
using Object = UnityEngine.Object;

public class RDF
{
    private SyncObjects _syncObjects;
    private string Repository;
    IGraph g;
    INode rootNode;
    private static RDF _rdf;

    // public RDF(string repo)
    // {
    //     Repository = repo;
    //     g = new Graph();
    // }
    // public RDF(SyncObjects syncObjects)
    // {
    //     _syncObjects = syncObjects;
    //     g = new Graph();
    // }
    private RDF()
    {
        
    }

    public static RDF Instance()
    {
        if(_rdf==null)
        {
            _rdf = new RDF();
        }
        return _rdf;
    }

    public void setGraph(ref IGraph graph, string rootUri="")
    {
        g = graph;
    }

    public void createRootNode(string rootUri)
    {
        rootNode = g.CreateUriNode(new Uri(rootUri));
    }

    public void UpdateAllSceneObjects(ref IGraph graph, string rootUri)
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in allObjects)
        {
            string type = getType(gameObject.name);
            if (type != null)
            {
                UpdateRDFRepresentation(gameObject, type);
            }
        }
    }
    private string getType(string name)
    {
        foreach (string objectOfInterest in _syncObjects.objectsOfInterest)
        {
            if (name.ToLower().Contains(objectOfInterest.ToLower()))
            {
                return (objectOfInterest!="Rack")?objectOfInterest:"Shelf";
            }
        }
        return null;
    }

    public void UpdateRDFRepresentation(GameObject obj, string type)
    {
        // INode _contains = graph.CreateUriNode(new Uri("http://www.dfki.de/unity-ns#contains"));
        INode _contains = g.CreateUriNode(new Uri(RdfSpecsHelper.RdfObject));
        Uri _string = new Uri(XmlSpecsHelper.XmlSchemaDataTypeString);
        INode s = g.CreateBlankNode();

        INode _rdftype = g.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        INode _type = g.CreateLiteralNode(type, _string);

        
        INode _label = g.CreateUriNode(new Uri(OntologyHelper.PropertyLabel));
        INode _name = g.CreateLiteralNode(obj.name,_string);
        
        INode _positionNode = g.CreateUriNode(new Uri("http://www.dfki.de/unity-ns#position"));
        INode _position = g.CreateLiteralNode(obj.transform.position.ToString(), _string);
        
        INode _rotationNode = g.CreateUriNode(new Uri("http://www.dfki.de/unity-ns#rotation"));
        INode _rotation = g.CreateLiteralNode(obj.transform.rotation.ToString(), _string);

        INode _forwardNode = g.CreateUriNode(new Uri("http://www.dfki.de/unity-ns#forward"));
        INode _forward = g.CreateLiteralNode(obj.transform.forward.ToString(), _string);

        g.Assert(new Triple(rootNode, _contains, s));
        g.Assert(new Triple(s, _rdftype, _type));
        g.Assert(new Triple(s, _label, _name));
        g.Assert(new Triple(s, _positionNode, _position));
        g.Assert(new Triple(s, _rotationNode, _rotation));
        g.Assert(new Triple(s, _forwardNode, _forward));
    }
}

public class SyncObjects : MonoBehaviour
{
    public string Repository = "http://localhost:8090/rdf4j/repositories/dummy_knowledge";
    public string rdf_server = "http://localhost:7200";
    public string repo_name = "dummy_knowledge";
    public string graph_name_uri = "https://www.openrdf.org/schema/sesame#nil";
    [FormerlySerializedAs("_objectsOfInterest")] public List<string> objectsOfInterest = new List<string>{ "Box", "Shelf", "Rack", "Worker", "Drone"};
    private readonly RDF _rdf = RDF.Instance();

    public RDF Rdf
    {
        get { return _rdf; }
    }

    public void SyncAll()
    {
        SesameHttpProtocolVersion6Connector graphDB =
            new SesameHttpProtocolVersion6Connector(rdf_server, repo_name);
        IGraph existingGraph = new Graph();
        graphDB.LoadGraph(existingGraph, graph_name_uri);
        graphDB.DeleteGraph(graph_name_uri);
        Rdf.setGraph(ref existingGraph);
        Rdf.createRootNode(graph_name_uri);
        Rdf.UpdateAllSceneObjects(ref existingGraph, graph_name_uri);
        graphDB.SaveGraph(existingGraph);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
