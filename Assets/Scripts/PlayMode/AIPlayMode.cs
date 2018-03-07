using System;
using System.IO;
#if ENABLE_TENSORFLOW
using TensorFlow;
#endif
using UnityEngine;

namespace FlappyBird
{
    public class AIPlayMode : BasePlayMode, IPlayMode
    {
#if ENABLE_TENSORFLOW
        private TFModel _tfTrainingModel;
        private TFModel _tfPreprocessingModel;
        private TFTensor inputTensor = null;
#endif

        private const int TARGET_FRAME_SIZE = 84;
        private const string TF_TRAINING_MODEL_DIR = "TFModel";
        private const string TF_TRAINING_MODEL_PATH = "TFModel/training_model.bytes";
        private const string TF_PREPROCESSING_GRAPH_DEF_PATH = "TFModel/image_resize_graph_def.pb";

        private const string PREPROCESSING_INPUT_NAME = "Input/input_x";
        private const string PREPROCESSING_OUTPUT_NAME = "Target/stacked_x";
        private const string PREPROCESSING_NEXT_INPUT_NAME = "NextTarget/input_stacked_x";
        private const string PREPROCESSING_NEXT_OUTPUT_NAME = "NextTarget/output_stacked_x";
        private const string NET_INPUT_NAME = "MainQNetwork/Input/x";
        private const string NET_OUTPUT_NAME = "MainQNetwork/Qout/QValue";
        private byte[] _binaryData;

        private void Awake()
        {
#if ENABLE_TENSORFLOW
            _tfTrainingModel = new TFModel();
            _tfPreprocessingModel = new TFModel();

            if(!File.Exists(Path.Combine(Application.persistentDataPath, TF_TRAINING_MODEL_DIR)))
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, TF_TRAINING_MODEL_DIR));
#endif
        }

        private void Start()
        {
            Logger.Print("AIPlayMode.Start");
        }

        private new void Update()
        {

        }

        private void FixedUpdate()
        {
#if ENABLE_TENSORFLOW
            if (_isPlaying && _tfTrainingModel.IsLoaded && _tfPreprocessingModel.IsLoaded)
            {
                if (GameManager.Instance.frameRecorder.IsCaptured)
                {
                    GameManager.Instance.frameRecorder.IsCaptured = false;
                    double startTime = DateTime.Now.Ticks / 10000.0;
                    Bird.Action action = GetAction();
                    double endTime = DateTime.Now.Ticks / 10000.0;
                    GameManager.Instance.bird.Flap(action);
                }
            }
#endif
        }

        private Bird.Action GetAction()
        {
#if ENABLE_TENSORFLOW
            try
            {
                double startTime = DateTime.Now.Ticks / 10000.0;
                byte[] binaryData = GetFrameBinaryData();
                double endTime = DateTime.Now.Ticks / 10000.0;
                if (inputTensor == null)
                {
                    startTime = DateTime.Now.Ticks / 10000.0;
                    inputTensor = _tfPreprocessingModel.GetOutput(PREPROCESSING_INPUT_NAME, PREPROCESSING_OUTPUT_NAME, binaryData);
                    endTime = DateTime.Now.Ticks / 10000.0;
                }
                else
                {
                    startTime = DateTime.Now.Ticks / 10000.0;
                    inputTensor = _tfPreprocessingModel.GetNextState(
                                                    PREPROCESSING_INPUT_NAME,
                                                    binaryData,
                                                    PREPROCESSING_NEXT_INPUT_NAME,
                                                    PREPROCESSING_NEXT_OUTPUT_NAME,
                                                    inputTensor);
                    endTime = DateTime.Now.Ticks / 10000.0;
                }
                startTime = DateTime.Now.Ticks / 10000.0;
                float[,] qvalues = _tfTrainingModel.GetValue(NET_INPUT_NAME, NET_OUTPUT_NAME, inputTensor);
                float q1 = qvalues[0, 0];
                float q2 = qvalues[0, 1];
                return q2 > q1 ? Bird.Action.Flap : Bird.Action.Idle;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return Bird.Action.Idle;
            }
#else
            return Bird.Action.Idle;
#endif
        }

        private byte[] GetFrameBinaryData()
        {
            var frameRecorder = GameManager.Instance.frameRecorder;
            byte[] imageData = frameRecorder.GetFrameImageData();
            int width = frameRecorder.CaptureWidth;
            int height = frameRecorder.CaptureHeight;
            if (_binaryData == null)
                _binaryData = new byte[width * height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int index = j + i * width;
                    byte r = imageData[3 * index];
                    byte g = imageData[3 * index + 1];
                    byte b = imageData[3 * index + 2];
                    int grayValue = Algorithm.Rgb2Gray(r, g, b);
                    _binaryData[index] = grayValue > 0 ? (byte)255 : (byte)0;
                }
            }
            return _binaryData;
        }

        public new void Play()
        {
#if ENABLE_TENSORFLOW
            inputTensor = null;
            GameManager.Instance.frameRecorder.BeginRecording();
            LoadTFTrainingModel();
            LoadTFPreprocessingGraph();
#endif
            base.Play();
        }

        private new void Stop()
        {
            GameManager.Instance.frameRecorder.EndRecording();
            base.Stop();
        }

        private bool LoadTFTrainingModel()
        {
#if ENABLE_TENSORFLOW
            if (_tfTrainingModel == null || _tfTrainingModel.IsLoaded)
                return false;
            if (File.Exists(Path.Combine(Application.persistentDataPath, TF_TRAINING_MODEL_PATH)))
                return _tfTrainingModel.LoadModel(Path.Combine(Application.persistentDataPath, TF_TRAINING_MODEL_PATH));
            else
                return _tfTrainingModel.LoadModel(Path.Combine(Application.streamingAssetsPath, TF_TRAINING_MODEL_PATH));
#else
            return false;
#endif
        }

        private bool LoadTFPreprocessingGraph()
        {
#if ENABLE_TENSORFLOW
            if (_tfPreprocessingModel == null || _tfPreprocessingModel.IsLoaded)
                return false;
            if (File.Exists(Path.Combine(Application.persistentDataPath, TF_PREPROCESSING_GRAPH_DEF_PATH)))
                return _tfPreprocessingModel.LoadGraphDef(Path.Combine(Application.persistentDataPath, TF_PREPROCESSING_GRAPH_DEF_PATH));
            else
                return _tfPreprocessingModel.LoadGraphDef(Path.Combine(Application.streamingAssetsPath, TF_PREPROCESSING_GRAPH_DEF_PATH));
#else
            return false;
#endif
        }
    }
}