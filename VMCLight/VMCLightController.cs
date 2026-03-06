using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VMCLight.Configuration;
using VMCLight.Osc;
using VMCLight.UI;

namespace VMCLight;

public class VMCLightController :MonoBehaviour
{
    public LightData ActiveLightData= new LightData();
    private List<SendTask> _sendTasks = new List<SendTask>();
    public bool inGameCoreScene = false;
    
    internal ModMainFlowCoordinator VMCLightMainFlowCoordinator;
    
    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
        SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
    }
    
    private void Start()
    {
        AddLightData(0);
        AddSendTask(ActiveLightData, PluginConfig.Instance.VMCProtocolAddress, PluginConfig.Instance.VMCProtocolPort);
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
                inGameCoreScene = true;
                break;
            default:
                inGameCoreScene = false;
                ActiveLightData.Color = PluginConfig.Instance.BlendColor;
                break;
        }
        Plugin.Log.Notice($"SceneChanged : {next.name}, inGameCoreScene : {inGameCoreScene}");
    }

    public void VMCProtocolReconnect()
    {
        RemoveTask(ActiveLightData);
        AddSendTask(ActiveLightData, PluginConfig.Instance.VMCProtocolAddress, PluginConfig.Instance.VMCProtocolPort);
    }
    
    public void AddLightData(int lightId)
    {
        ActiveLightData.LightId = lightId;
        ActiveLightData.Color = new Color(1f, 1f, 1f, 1f);
        ActiveLightData.Position = new Vector3(0f, 1.5453f, 0f);
        ActiveLightData.Rotation = Quaternion.Euler(130f, 43f, 75f);
    }
    
    internal void AddSendTask(LightData data, string address = "127.0.0.1", int port = 39540)
    {
        SendTask sendTask = new SendTask();
        sendTask.data = data;
        sendTask.client = new OscClient(address, port);
        if (sendTask.client != null)
        {
            Plugin.Log.Notice($"Instance of OscClient {address}:{port} Starting.");
            _sendTasks.Add(sendTask);
        }
        else
            Plugin.Log.Error($"Instance of OscClient Not Starting.");
    }
    
    internal void RemoveTask(LightData data)
    {
        foreach(SendTask sendTask in _sendTasks)
        {
            if (sendTask.data.LightId == data.LightId)
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
                    sendTask.client.Send("/VMC/Ext/Light", "Light", new float[] {
                        sendTask.data.Position.x, sendTask.data.Position.y, sendTask.data.Position.z,
                        sendTask.data.Rotation.x, sendTask.data.Rotation.y, sendTask.data.Rotation.z, sendTask.data.Rotation.w,
                        sendTask.data.Color.r, sendTask.data.Color.g, sendTask.data.Color.b, sendTask.data.Color.a
                    });
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