# LumaX - Home Automation System

## Overview

**LumaX** is a scalable and efficient home automation system that allows users to control the lighting and monitor the temperature of their rooms through an intuitive web-based dashboard. The system integrates a variety of technologies to provide real-time control and monitoring, making it ideal for both home and industrial-level automation.

## Features

- **Real-time Temperature Monitoring**: See live temperature updates from connected rooms through the dashboard.
- **Light Control**: Control the lighting in each room from anywhere using the web interface.
- **Scalability**: Designed to grow with additional functionalities and scale to larger industrial setups.

## Technology Stack

### Frontend
- **[Angular (v18)]**
- **Real-time Updates**: SignalR integration to provide instant feedback and control in the dashboard.
  
### Backend
- **Framework**: [ASP.NET Core (latest version)](https://dotnet.microsoft.com/apps/aspnet)
  
### Communication
  - **MQTT**: Using [MQTTnet (v4.3.7.1207)] for efficient communication between the backend and the ESP32 hardware.
  - **SignalR**: Real-time data transmission between frontend and backend.
  
### Database
- **MySQL**

### Hardware
- **ESP32**: Microcontroller that handles sensors and devices for temperature monitoring and light control.

