# LumaX - Home Automation System

## Overview

**LumaX** is a scalable and efficient home automation system that allows users to control the lighting and monitor the temperature of their rooms through a web-based dashboard. The system integrates a variety of technologies to provide real-time control and monitoring.

## Features

- **Real-time Temperature Monitoring**: See live temperature updates from connected rooms through the dashboard.
- **Light Control**: Control the lighting in each room from anywhere using the web interface.
- **User Authentication & Authorization**: Implemented with ASP.NET Core Identity to ensure secure access to the system.
- **Scalability**: Designed to grow with additional functionalities and scale to larger industrial setups.

## Technology Stack

### Frontend
- **Angular (v18)**
  
### Backend
- **ASP.NET Core**
  - **MQTT** protocol for communication between ESP32 and backend.  
  - **SignalR** for real-time data updates.  
  - **ASP.NET Core Identity** for authentication and authorization.
  
### Communication
  - **MQTT**: Communication between the backend(ASP.NET Core) and the ESP32 hardware.
  - **SignalR**: Real-time data transmission between frontend(Anguler) and backend(ASP.NET Core).
  
### Database
- **MySQL**, managed within a Docker environment.

### Hardware
- **ESP32**: Microcontroller that handles sensors and devices for temperature monitoring and light control.

