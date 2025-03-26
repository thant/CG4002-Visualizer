/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;




/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace M2MqttUnity.Examples
{
    using Newtonsoft.Json;


    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    using UnityEngine;

    public class PlayerData
    {
        public string playerId;
        public string action;
        public int hp;
        public int bullets;
        public int bombs;
        public int shieldHp;
        public int shields;
        public int deaths;
        public int seeOpponent;
    };

    public class SendMessageData
    {
        public string topic;
        public string playerId;
        public string action;
        public int isActive;
        public int isVisible;
    };

    public class ReceiveMessageData
    {
        public string topic;
        public string message;
    };

    // Usage Example


    public class M2MqttUnityTest : M2MqttUnityClient
    {
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;
        [Header("User Interface")]
        public InputField consoleInputField;
        public Toggle encryptedToggle;
        public InputField addressInputField;
        public InputField portInputField;
        public Button connectButton;
        public Button disconnectButton;
        public Button testPublishButton;
        public Button clearButton;
        public ARLaserTagUI arLaserTagUI;

        public const string MQTT_TOPIC = "cg4002_b15";

        private List<string> eventMessages = new ();
        private bool updateUI = false;

        private SendMessageData currSendMessage = new ();
        private ReceiveMessageData currReceiveMessage = new ();

        public void PublishMessage(string action = "", int isActive = 0, int isVisible = 0){
    // Create a new message with the given data
        SendMessageData player = new()
        {
            topic = "visualiser/mqtt_server",
            playerId = "1",
            action = action,
            isActive = isActive,
            isVisible = isVisible,
        };
        var messageJson = JsonUtility.ToJson(player);
        Debug.Log(messageJson);
        client.Publish(MQTT_TOPIC, System.Text.Encoding.UTF8.GetBytes(messageJson), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        AddUiMessage("Message published.");
        }

        public void SetBrokerAddress(string brokerAddress)
        {
            if (addressInputField && !updateUI)
            {
                this.brokerAddress = brokerAddress;
            }
        }

        public void SetBrokerPort(string brokerPort)
        {
            if (portInputField && !updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        public void SetUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text = msg;
                updateUI = true;
            }
        }

        public void AddUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text += msg + "\n";
                updateUI = true;
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SetUiMessage("Connected to broker on " + brokerAddress + "\n");

            if (autoTest)
            {
                PublishMessage();
            }
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { MQTT_TOPIC }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { MQTT_TOPIC });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            AddUiMessage("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            AddUiMessage("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            AddUiMessage("CONNECTION LOST!");
        }

        private void UpdateUI()
        {
            if (client == null)
            {
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                    disconnectButton.interactable = false;
                    testPublishButton.interactable = false;
                }
            }
            else
            {
                if (testPublishButton != null)
                {
                    testPublishButton.interactable = client.IsConnected;
                }
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = client.IsConnected;
                }
                if (connectButton != null)
                {
                    connectButton.interactable = !client.IsConnected;
                }
            }
            if (addressInputField != null && connectButton != null)
            {
                addressInputField.interactable = connectButton.interactable;
                addressInputField.text = brokerAddress;
            }
            if (portInputField != null && connectButton != null)
            {
                portInputField.interactable = connectButton.interactable;
                portInputField.text = brokerPort.ToString();
            }
            if (encryptedToggle != null && connectButton != null)
            {
                encryptedToggle.interactable = connectButton.interactable;
                encryptedToggle.isOn = isEncrypted;
            }
            if (clearButton != null && connectButton != null)
            {
                clearButton.interactable = connectButton.interactable;
            }
            updateUI = false;
        }

        protected override void Start()
        {
            SetUiMessage("Ready.");
            updateUI = true;
            base.Start();
        }

        public int isVisible;



        public void Visibility(int newvalue)
        {
            isVisible = newvalue;
        }

        protected override void DecodeMessage(string topic, byte[] message)
{
    string msg = System.Text.Encoding.UTF8.GetString(message);

    // If the message is empty or doesn't have expected data, skip it.
    if (string.IsNullOrEmpty(msg) || msg == "{}")
    {
        Debug.Log("Received empty message, skipping.");
        return; // Exit the method early
    }

    // Log the received message for debugging purposes
    Debug.Log($"Received message on topic '{topic}': {msg}");

    JsonUtility.FromJsonOverwrite(msg, currSendMessage);

    // Check if the topic matches the expected ones
    if (currSendMessage.topic != "visualiser/mqtt_server")
    {
        StoreMessage(msg);
    }

    if (topic == MQTT_TOPIC)
    {
        if (autoTest)
        {
            autoTest = false;
            Disconnect();
        }
    }

    ProcessMessage(msg);
}


 public int currID = 1;

 public void setP2(){
    this.currID = 2;
    }


        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

    private void ProcessMessage(string msg)
        {
            // Add raw message to the UI for debugging
            AddUiMessage("Received: " + msg);
            string temptopic = ExtractJsonField(msg, "topic").Split(",")[0].Trim('"');
            if (temptopic == "client/visualiser/action")
                {
                    string messagePart = ExtractJsonField(msg, "message");
                    string action = ExtractStringField(messagePart, "action");
                    arLaserTagUI.UpdateAction(action);
                    int isActive = GameObject.Find("collisionchecker").GetComponent<Flipflop>().isActive;
                    PublishMessage(action, isActive, isVisible);
                    Debug.Log($"Published isActive: {isActive}, isVisible: {isVisible}");
                }
            else if (temptopic == "client/visualiser/gamestate")
    {
    string p1Data = ExtractJsonField(msg, "p1");
    string p2Data = ExtractJsonField(msg, "p2");

    if (p1Data == null || p2Data == null)
    {
        Debug.LogError("Invalid player data in game state message.");
        return;
    }

    // Extract values for Player 1
    int p1Hp = ExtractIntField(p1Data, "hp");
    int p1Bullets = ExtractIntField(p1Data, "bullets");
    int p1Bombs = ExtractIntField(p1Data, "bombs");
    int p1ShieldHp = ExtractIntField(p1Data, "shield_hp");
    int p1Deaths = ExtractIntField(p1Data, "deaths");
    int p1Shields = ExtractIntField(p1Data, "shields");

    // Extract values for Player 2
    int p2Hp = ExtractIntField(p2Data, "hp");
    int p2Bullets = ExtractIntField(p2Data, "bullets");
    int p2Bombs = ExtractIntField(p2Data, "bombs");
    int p2ShieldHp = ExtractIntField(p2Data, "shield_hp");
    int p2Deaths = ExtractIntField(p2Data, "deaths");
    int p2Shields = ExtractIntField(p2Data, "shields");

    if (currID == 1){
        arLaserTagUI.UpdatePlayerStats(p1Hp, p1Bullets, p1ShieldHp, p1Shields, p1Bombs);
        arLaserTagUI.UpdateOpponentStats(p2Hp, p2Bullets, p2ShieldHp, p2Shields, p2Bombs);
    }
    else{
        arLaserTagUI.UpdateOpponentStats(p1Hp, p1Bullets, p1ShieldHp, p1Shields, p1Bombs);
        arLaserTagUI.UpdatePlayerStats(p2Hp, p2Bullets, p2ShieldHp, p2Shields, p2Bombs);
    }
    }
        }

// Helper method to extract the value of a JSON field (as string)
private string ExtractJsonField(string json, string field)
{
    string pattern = $"\"{field}\":";
    int startIndex = json.IndexOf(pattern);
    if (startIndex == -1) return null; // Field not found

    startIndex += pattern.Length;
    int endIndex = json.IndexOf("}", startIndex);
    if (endIndex == -1) endIndex = json.IndexOf(",", startIndex);

    // If no comma is found, ensure that we read until the closing curly brace
    if (endIndex == -1) endIndex = json.Length;

    return json.Substring(startIndex, endIndex - startIndex).Trim();
}





// Helper method to extract an integer value from the JSON field
private int ExtractIntField(string json, string field)
{
    string value = ExtractJsonField(json, field);
    return int.TryParse(value, out int result) ? result : 0;
}

// Helper method to extract a string value from the JSON field
private string ExtractStringField(string json, string field)
{
    string value = ExtractJsonField(json, field);
    return value != null ? value.Trim('"') : null;
}




        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()
            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                   //ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
            if (updateUI)
            {
                UpdateUI();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }
    }
}
