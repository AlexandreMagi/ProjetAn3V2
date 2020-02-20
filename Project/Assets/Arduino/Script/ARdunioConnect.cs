using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Win32;
using System.IO.Ports;
using System;

public class ARdunioConnect : MonoBehaviour
{

    wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen your computer and hardware.

    [Tooltip("SerialPort of your device.")]
    public string portName = "COM8";

    [Tooltip("Baudrate")]
    public int baudRate = 250000;


    [Tooltip("Timeout")]
    public int ReadTimeout = 20;

    [Tooltip("QueueLenght")]
    public int QueueLenght = 1;

    private string lastData = null;

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
    }

    public static string AutodetectArduinoPort(string deviceNameContains = "Arduino", bool debug = false)
    {
        List<string> comports = new List<string>();
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        string temp;
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                if (s.Contains("VID") && s.Contains("PID"))
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);

                        if(debug) Debug.Log((string)rk5.GetValue("FriendlyName"));

                        if ((temp = (string)rk5.GetValue("FriendlyName")) != null && temp.Contains(deviceNameContains))
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                comports.Add(temp);
                            }
                        }
                    }
                }
            }
        }

        if (comports.Count > 0)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comports.Contains(s))
                    return s;
            }
        }

        return "COM9";
    }


    // Start is called before the first frame update
    void Start()
    {
        portName = ARdunioConnect.AutodetectArduinoPort("Genuino");
        Debug.LogWarning("Device Detected on Port : " + portName);

        myDevice.set(portName, baudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
                                                                    //                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
        myDevice.connect(); // This method open the Serial communication with the vars previously given.
    }

    // Update is called once per frame
    void Update()
    {
        string data = myDevice.readQueue();
        if (data != null)
        {
            lastData = data;
            //print("Data from Arduino : " + data);
        }
        
    }

    public string getLastDataFromDevice()
    {
        return lastData;
    }

    void OnApplicationQuit()
    { // close the Thread and Serial Port
        myDevice.close();
    }
    private void OnDestroy()
    {
        myDevice.close();
    }

}
