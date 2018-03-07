using System;
using System.IO;

#if ENABLE_TENSORFLOW
using TensorFlow;
using UnityEngine;

public class TFModel
{
    public bool IsLoaded
    {
        get
        {
            return _bModelLoaded;
        }
    }

    private TFGraph _tfGraph;
    private TFSession _session;
    private bool _bModelLoaded = false;

    public TFModel()
    {
        _tfGraph = new TFGraph();
    }

    public bool LoadModel(string modelPath)
    {
        if (_bModelLoaded)
            return true;
        try
        {
            Logger.Print("Load TF model from '{0}'", modelPath);
            byte[] bytes = null;
            if (modelPath.Contains("://"))
            {
                WWW www = new WWW(modelPath);
                while (!www.isDone) ;
                bytes = www.bytes;
            }
            else
            {
                bytes = File.ReadAllBytes(modelPath);
            }
            if (bytes == null || bytes.Length == 0)
            {
                Logger.Error("TFModel.LoadModelFromPath - failed to load tf model from '{0}'", modelPath);
                return false;
            }
            Logger.Print("Try to import data bytes, length: {0}", bytes.Length);
            _tfGraph.Import(bytes);

            Logger.Print("Create session...");
            _session = new TFSession(_tfGraph);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return false;
        }
        Logger.Print("Load success.");
        _bModelLoaded = true;
        return true;
    }

    public bool LoadGraphDef(string graphPath)
    {
        if (_bModelLoaded)
            return true;
        try
        {
            Logger.Print("TFModel.LoadGraphDef - Load TF graph def from '{0}'", graphPath);
            byte[] bytes = null;
            if (graphPath.Contains("://"))
            {
                WWW www = new WWW(graphPath);
                while (!www.isDone) ;
                bytes = www.bytes;
            }
            else
            {
                bytes = File.ReadAllBytes(graphPath);
            }
            if (bytes == null || bytes.Length == 0)
            {
                Logger.Error("TFModel.LoadGraphDef - failed to load tf model from '{0}'", graphPath);
                return false;
            }
            TFBuffer graphDef = new TFBuffer(bytes);
            Logger.Print("Try to import graphDef, bytes length: {0}", bytes.Length);

            _tfGraph.Import(graphDef);
            Logger.Print("Create session...");
            _session = new TFSession(_tfGraph);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return false;
        }
        Logger.Print("Load success.");
        _bModelLoaded = true;
        return true;
    }

    public float[,] GetValue(string inputNode, string outputNode, TFTensor intput)
    {
        if (!_bModelLoaded)
            return null;
        TFSession.Runner runner = _session.GetRunner();
        runner.AddInput(_tfGraph[inputNode][0], intput);
        runner.Fetch(_tfGraph[outputNode][0]);
        TFTensor[] output = runner.Run();
        float[,] value = (float[,])output[0].GetValue();
        return value;
    }

    public TFTensor GetOutput(string inputNode, string outputNode, byte[] inputData)
    {
        if (!_bModelLoaded)
            return null;

        var tensor = TFTensor.FromBuffer(new TFShape(1, inputData.Length), inputData, 0, inputData.Length);
        TFSession.Runner runner = _session.GetRunner();
        runner.AddInput(_tfGraph[inputNode][0], tensor);
        runner.Fetch(_tfGraph[outputNode][0]);
        TFTensor[] output = runner.Run();
        return output[0];
    }

    public TFTensor GetNextState(string currInputNode, byte[] inputData, string nextStateInputNode, string nextStateOutputNode, TFTensor nextStateTensor)
    {
        if (!_bModelLoaded)
            return null;

        var runner = _session.GetRunner();
        var currStateTensor = TFTensor.FromBuffer(new TFShape(1, inputData.Length), inputData, 0, inputData.Length);

        runner.AddInput(_tfGraph[currInputNode][0], currStateTensor);
        runner.AddInput(_tfGraph[nextStateInputNode][0], nextStateTensor);
        runner.Fetch(_tfGraph[nextStateOutputNode][0]);
        TFTensor[] output = runner.Run();
        return output[0];
    }
}
#endif