﻿using MilestoneDriverExample;
using System;
using VideoOS.Platform;

namespace ConsoleApp4
{
    class Program
    {
         static void Main(string[] args)
        {
            MilestoneDriver driver = new MilestoneDriver();

            driver.Initialize();

            string serverAddress = "http://172.86.106.216";
            string username = "Administrator";
            string password = "X7vcYc1Cc11QPG";

            if (driver.Connect(serverAddress, username, password))
            {
                Console.WriteLine("Connected to Milestone server.");

                // Get the first camera
                Item camera = driver.GetFirstCamera();
                if (camera != null)
                {
                    Console.WriteLine("Camera found: " + camera.Name);
                    driver.StartLiveStreaming(camera);
                    Console.WriteLine("Press any key to stop streaming...");
                    Console.ReadKey();

                    // Stop streaming
                    driver.StopLiveStreaming();
                }
                else
                {
                    Console.WriteLine("No cameras found.");
                }

                // Log out
                driver.Logout(serverAddress);
            }
            else
            {
                Console.WriteLine("Failed to connect to Milestone server.");
            }
        }
    }
}
