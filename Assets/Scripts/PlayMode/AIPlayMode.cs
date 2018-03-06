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
        private const string TF_TRAINING_MODEL_PATH = "TFModel/graph_def.bytes";
        private const string TF_PREPROCESSING_GRAPH_DEF_PATH = "TFModel/image_preprocess_graph_def.pb";
        private const string PREPROCESSING_INPUT_NAME = "Input/input_x";
        private const string PREPROCESSING_OUTPUT_NAME = "Target/stacked_x";
        private const string PREPROCESSING_NEXT_INPUT_NAME = "NextTarget/input_stacked_x";
        private const string PREPROCESSING_NEXT_OUTPUT_NAME = "NextTarget/output_stacked_x";
        private const string NET_INPUT_NAME = "MainQNetwork/Input/x";
        private const string NET_OUTPUT_NAME = "MainQNetwork/Qout/QValue";
        private TFTensor inputTensor = null;
#endif

        private void Awake()
        {
#if ENABLE_TENSORFLOW
            _tfTrainingModel = new TFModel();
            _tfPreprocessingModel = new TFModel();
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
                    Bird.Action action = GetAction();
                    GameManager.Instance.bird.Flap(action);
                }
            }
#endif
        }

        private Bird.Action GetAction()
        {
#if ENABLE_TENSORFLOW
            byte[] imageData = GameManager.Instance.frameRecorder.GetFrameImageData();
            if (inputTensor == null)
            {
                inputTensor = _tfPreprocessingModel.GetOutput(PREPROCESSING_INPUT_NAME, PREPROCESSING_OUTPUT_NAME, imageData);
            }
            else
            {
                inputTensor = _tfPreprocessingModel.GetNextState(
                                                PREPROCESSING_INPUT_NAME,
                                                imageData,
                                                PREPROCESSING_NEXT_INPUT_NAME, 
                                                PREPROCESSING_NEXT_OUTPUT_NAME, 
                                                inputTensor);
            }
            float[,] qvalues = _tfTrainingModel.GetValue(NET_INPUT_NAME, NET_OUTPUT_NAME, inputTensor);
            float q1 = qvalues[0, 0];
            float q2 = qvalues[0, 1];
            return q2 > q1 ? Bird.Action.Flap : Bird.Action.Idle;
#else
            return Bird.Action.Idle;
#endif
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