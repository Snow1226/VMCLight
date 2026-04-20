using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VMCLight.Configuration;
using VMCLight.Osc;
using VMCLight.UI;
using VMCLight.HarmonyPatches;
namespace VMCLight;

public class VMCLightController :MonoBehaviour
{
    public LightData ActiveLightData= new LightData();
    public LightData ActiveLeftSaberLightData = new LightData();
    public LightData ActiveRightSaberLightData = new LightData();
    
    private List<SendTask> _sendTasks = new List<SendTask>();
    public bool inGameCoreScene = false;
    
    internal ModMainFlowCoordinator VMCLightMainFlowCoordinator;
    
    private GameObject _leftSaber;
    private GameObject _rightSaber;

    
    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
        SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
    }
    
    private void Start()
    {
        ActiveLightData.LigthType = LightTypeEnum.Directional;
        ActiveLightData.Color = new Color(1f, 1f, 1f, 1f);
        ActiveLightData.Position = new Vector3(0f, 1.5453f, 0f);
        ActiveLightData.Rotation = Quaternion.Euler(130f, 43f, 75f);
        
        AddSendTask(ActiveLightData, PluginConfig.Instance.VMCLightProtocolAddress, PluginConfig.Instance.VMCLightProtocolPort);

        ActiveLeftSaberLightData.LigthType = LightTypeEnum.LeftSaber;
        ActiveLeftSaberLightData.Color = new Color(1f, 1f, 1f, 1f);
        ActiveLeftSaberLightData.Position = Vector3.zero;
        ActiveLeftSaberLightData.Rotation = Quaternion.identity;
        
        AddSendTask(ActiveLeftSaberLightData, PluginConfig.Instance.VMCLightProtocolAddress, PluginConfig.Instance.VMCLightProtocolPort);
        
        ActiveRightSaberLightData.LigthType = LightTypeEnum.RightSaber;
        ActiveRightSaberLightData.Color = new Color(1f, 1f, 1f, 1f);
        ActiveRightSaberLightData.Position = Vector3.zero;
        ActiveRightSaberLightData.Rotation = Quaternion.identity;
        
        AddSendTask(ActiveRightSaberLightData, PluginConfig.Instance.VMCLightProtocolAddress, PluginConfig.Instance.VMCLightProtocolPort);

    }

    private void Update()
    {
        if (MenuControllerPatch.EnableVRController)
        {
            ActiveLeftSaberLightData.Position = MenuControllerPatch.LeftSaber.transform.position;
            ActiveLeftSaberLightData.Rotation = MenuControllerPatch.LeftSaber.transform.rotation;
            ActiveRightSaberLightData.Position = MenuControllerPatch.RightSaber.transform.position;
            ActiveRightSaberLightData.Rotation = MenuControllerPatch.RightSaber.transform.rotation;
        }
        else if(_leftSaber && _rightSaber)
        {
            ActiveLeftSaberLightData.Position = _leftSaber.transform.position;
            ActiveLeftSaberLightData.Rotation = _leftSaber.transform.rotation;
            ActiveRightSaberLightData.Position = _rightSaber.transform.position;
            ActiveRightSaberLightData.Rotation = _rightSaber.transform.rotation;
        }

        ActiveLeftSaberLightData.Color = ColorManagerPatch.SaberColorA;
        ActiveRightSaberLightData.Color = ColorManagerPatch.SaberColorB;
    }
    private void LateUpdate()
    {
        Task.Run(() => SendData());
    }

    private void OnActiveSceneChanged(Scene current, Scene next)
    {
        switch (next.name)
        {
            case "GameCore":
                StartCoroutine(DelayedFindController());
                inGameCoreScene = true;
                break;
            default:
                inGameCoreScene = false;
                ActiveLightData.Color = PluginConfig.Instance.BlendColor;
                break;
        }
    }

    private IEnumerator DelayedFindController()
    {
        while (_leftSaber == null && _rightSaber == null)
        {
            yield return null;
            _leftSaber = GameObject.Find("LeftSaber");
            _rightSaber = GameObject.Find("RightSaber");
            if (_leftSaber)
                Plugin.Log.Notice($"LeftSaber {_leftSaber.name} is active.");
            if (_rightSaber)
                Plugin.Log.Notice($"RightSaber {_rightSaber.name} is active.");
        }
    }

    public void VMCProtocolReconnect()
    {
        RemoveTask(ActiveLightData);
        AddSendTask(ActiveLightData, PluginConfig.Instance.VMCLightProtocolAddress, PluginConfig.Instance.VMCLightProtocolPort);
    }
    
    internal void AddSendTask(LightData data, string address, int port)
    {
        SendTask sendTask = new SendTask();
        sendTask.data = data;
        sendTask.client = new OscClient(address, port);
        if (sendTask.client != null)
        {
            Plugin.Log.Notice($"{data.LigthType.ToString()} Instance of OscClient {address}:{port} Starting.");
            _sendTasks.Add(sendTask);
        }
        else
            Plugin.Log.Error($"Instance of OscClient Not Starting.");
    }
    
    internal void RemoveTask(LightData data)
    {
        foreach(SendTask sendTask in _sendTasks)
        {
            if (sendTask.data.LigthType == data.LigthType)
            {
                _sendTasks.Remove(sendTask);
                break;
            }
        }
    }
    
    private async Task SendData()
    {
        await Task.Run(() => {
            try
            {
                foreach(SendTask sendTask in _sendTasks)
                {
                    switch (sendTask.data.LigthType)
                    {
                        case LightTypeEnum.Directional:
                            sendTask.client.Send("/VMC/Ext/DirectionalLight", "Light", new float[] {
                                sendTask.data.Position.x, sendTask.data.Position.y, sendTask.data.Position.z,
                                sendTask.data.Rotation.x, sendTask.data.Rotation.y, sendTask.data.Rotation.z, sendTask.data.Rotation.w,
                                sendTask.data.Color.r, sendTask.data.Color.g, sendTask.data.Color.b, sendTask.data.Color.a
                            });
                            break;
                        case LightTypeEnum.LeftSaber:
                            
                            sendTask.client.Send("/VMC/Ext/LeftSaber", "Light", new float[] {
                                sendTask.data.Position.x, sendTask.data.Position.y, sendTask.data.Position.z,
                                sendTask.data.Rotation.x, sendTask.data.Rotation.y, sendTask.data.Rotation.z, sendTask.data.Rotation.w,
                                sendTask.data.Color.r, sendTask.data.Color.g, sendTask.data.Color.b, sendTask.data.Color.a
                            });
                            break;
                        case LightTypeEnum.RightSaber:
                            sendTask.client.Send("/VMC/Ext/RightSaber", "Light", new float[] {
                                sendTask.data.Position.x, sendTask.data.Position.y, sendTask.data.Position.z,
                                sendTask.data.Rotation.x, sendTask.data.Rotation.y, sendTask.data.Rotation.z, sendTask.data.Rotation.w,
                                sendTask.data.Color.r, sendTask.data.Color.g, sendTask.data.Color.b, sendTask.data.Color.a
                            });
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Error($"ExternalSender Thread : {e}");
            }
        });
    }
    
    internal class SendTask
    {
        internal LightData data;
        internal OscClient client;
    }
}