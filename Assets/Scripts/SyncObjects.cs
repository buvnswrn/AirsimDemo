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

    public RDF(string repo)
    {
        Repository = repo;
        g = new Graph();
    }
    public RDF(SyncObjects syncObjects)
    {
        _syncObjects = syncObjects;
        g = new Graph();
    }

    public void setGraph(ref IGraph graph, string rootUri="")
    {
        g = graph;
    }

    public void createRootNode(string rootUri)
    {
        rootNode = g.CreateUriNode(new Uri(rootUri));
    }

    public string GetAllSceneObjects()
    {
        StringBuilder RDFObjects = new StringBuilder();
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            string type = getType(obj.name);
            if (type!=null)
            {
                RDFObjects.Append((string)GetRDFRepresentation(obj,type));
            }
        }
        return RDFObjects.ToString();
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

    public void GetAllSceneObjectsRDF()
    {
        SesameHttpProtocolConnector sesame = new SesameHttpProtocolConnector("http://localhost:8090/rdf4j/repositories/", "dummy_knowledge");
        IUriNode ajanRDF = g.CreateUriNode(UriFactory.Create("http://www.ajan.de"));
        IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
        ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
        ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");
        g.Assert(new Triple(ajanRDF, says, helloWorld));
        g.Assert(new Triple(ajanRDF, says, bonjourMonde));
        var handler = new WriteToStoreHandler(sesame, 10);
        // RdfXmlWriter rdfXmlWriter = new RdfXmlWriter();
        // System.IO.StringWriter sw = new System.IO.StringWriter();
        // rdfXmlWriter.Save(g, sw);
        // String data = sw.ToString();
    }

    public void TestLoading()
    {
        // IGraph graph = new Graph();
        // Loader loader = new Loader();
        // loader.LoadGraph(graph, new Uri("http://localhost:7200/repositories/dummy_knowledge/statements"));
        // foreach (IUriNode uriNode in graph.Nodes.UriNodes())
        // {
        //     Debug.Log(uriNode.Uri.ToString());
        // }
        
        SesameHttpProtocolVersion6Connector graphDB =
            new SesameHttpProtocolVersion6Connector("http://localhost:8090/rdf4j", "dummy_knowledge");
        // graphDB.DeleteGraph("https://www.openrdf.org/schema/sesame#nil");
        IGraph newGraph = new Graph();
        graphDB.LoadGraph(newGraph, "https://www.openrdf.org/schema/sesame#nil");
        INode s = newGraph.CreateBlankNode();
        INode p = newGraph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        INode o = newGraph.CreateUriNode(new Uri("Http://example.org/Example1"));
        Triple t = new Triple(s, p, o);
        newGraph.Assert(t);
        // newGraph.BaseUri = new Uri("http://ajan.de/testing");
        Debug.Log(graphDB.DeleteSupported);
        Debug.Log(graphDB.ListGraphsSupported);
        Debug.Log(graphDB.UpdateSupported);
        graphDB.SaveGraph(newGraph);
        IGraph emptyGraph = new Graph();
        if(graphDB.UpdateSupported)
        {
            // graphDB.UpdateGraphAsync("https://www.openrdf.org/schema/sesame#nil", new Triple[] { t }, null,new CancellationToken(false));
        }
        if (graphDB.ListGraphsSupported)
        {
            foreach (Uri u in graphDB.ListGraphs())
            {
                Debug.Log(u);
            }
        }
        // graphDB.SaveGraph(graph);


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

    public string GetRDFRepresentation(GameObject obj, string type)
    {
        StringBuilder graph = new StringBuilder();
        string sceneObject = "<tcp://"+"localhost"+":"+"8090"+"/" + obj.GetInstanceID() + ">";
        graph.Append(sceneObject + " " + "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>" + " " +
                     "<http://www.dfki.de/unity-ns#"+type+"> .");
        graph.Append(sceneObject + " " + "<http://www.w3.org/2000/01/rdf-schema#label>" + " " + "'" + obj.name + "'" + ".");
        graph.Append(sceneObject + " " + "<http://www.w3.org/2000/01/rdf-schema#position>" + " " + "'" + obj.transform.position.ToString() + "'" + ".");
        return graph.ToString();
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

    public void UpdateRepository(string objects)
    {
        _syncObjects.StartCoroutine(Replace(objects));
    }

    private IEnumerator Replace(string objects)
    {
        WWWForm form = new WWWForm();
        form.AddField("update", "DELETE {?s ?p ?o} WHERE {?s ?p ?o}");
        using (UnityWebRequest www = UnityWebRequest.Post(Repository + "/statements", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                _syncObjects.StartCoroutine(Upload(objects));
            }
        }
    }

    public IEnumerator Upload(string objects)
    {
        WWWForm form = new WWWForm();
        form.AddField("update", "INSERT DATA {" + objects + "} ");
        using (UnityWebRequest www = UnityWebRequest.Post(Repository + "/statements", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}

public class SyncObjects : MonoBehaviour
{
    public string Repository = "http://localhost:8090/rdf4j/repositories/dummy_knowledge";
    public string rdf_server = "http://localhost:7200";
    public string repo_name = "dummy_knowledge";
    public string graph_name_uri = "https://www.openrdf.org/schema/sesame#nil";
    [FormerlySerializedAs("_objectsOfInterest")] public List<string> objectsOfInterest = new List<string>{ "Box", "Shelf", "Rack", "Worker", "Drone"};
    private readonly RDF _rdf;

    public SyncObjects()
    {
        _rdf = new RDF(this);
    }

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
