using System.IO;
#if ENABLE_TENSORFLOW

using TensorFlow;

public class TFUtils{

    public static TFTensor CreateTensorFromImageFile(string file, TFDataType destinationDataType = TFDataType.Float)
    {
        var contents = File.ReadAllBytes(file);
        return CreateTensor(contents, destinationDataType);
    }

    public static TFTensor CreateTensor(byte[] contents, TFDataType destinationDataType = TFDataType.Float)
    {
        // DecodeJpeg uses a scalar String-valued tensor as input.
        var tensor = TFTensor.CreateString(contents);

        TFGraph graph;
        TFOutput input, output;

        // Construct a graph to normalize the image
        ConstructGraphToNormalizeImage(out graph, out input, out output, destinationDataType);

        // Execute that graph to normalize this one image
        using (var session = new TFSession(graph))
        {
            var normalized = session.Run(
                     inputs: new[] { input },
                     inputValues: new[] { tensor },
                     outputs: new[] { output });

            return normalized[0];
        }
    }

    private static void ConstructGraphToNormalizeImage(out TFGraph graph, out TFOutput input, out TFOutput output, TFDataType destinationDataType = TFDataType.Float)
    {
        graph = new TFGraph();
        input = graph.Placeholder(TFDataType.String);
        const float Scale = 255.0f;
        output = graph.Cast(graph.Div(
            x: graph.ExpandDims(
                    input: graph.Cast(
                        graph.DecodeJpeg(contents: input, channels: 3), DstT: TFDataType.Float),
                    dim: graph.Const(0, "make_batch")),
            y: graph.Const(Scale, "scale")), destinationDataType);
    }
}
#endif
