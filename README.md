# Universal Smart Home

Universal Smart Home is a production-oriented, multi-protocol smart device management platform built with **C# and .NET 10 LTS**. It provides a single centralized control and monitoring surface across heterogeneous smart home ecosystems, independent Model Context Protocol (MCP) servers, Matter, and local Wi-Fi devices.

---

## High-Level Architecture

Universal Smart Home is designed as a modular monolith with strict separation of concerns, supporting two flexible deployment modes:

1. **Mode A: Windows-only Local Deployment**
   - WPF Desktop Application running MVVM.
   - Background host running device connectors on the local network.
   - Local SQLite database managed through Entity Framework Core.
   - Local ASP.NET Core & SignalR endpoints for real-time notifications.
2. **Mode B: Dedicated Home Server / Edge Deployment**
   - `UniversalSmartHome.Edge` running continuously on Linux or Windows home servers.
   - Connectors remain in the local home network with resilient heartbeat and reconnect backoff.
   - Desktop, web, or mobile clients connect remotely to the authenticated Edge API and SignalR hub.

### Solution Project Structure

```
UniversalSmartHome.sln
├── src/
│   ├── UniversalSmartHome.Domain                 # Pure domain models, entities, capabilities, commands, states
│   ├── UniversalSmartHome.Connectors.Abstractions# Contracts for device connectors, discovery, and execution
│   ├── UniversalSmartHome.Application            # Use cases, DTOs, device and connector managers, reconciliation
│   ├── UniversalSmartHome.Infrastructure         # EF Core persistence, SQLite, security, DPAPI, logging
│   ├── UniversalSmartHome.Connectors.HomeAssistant# Home Assistant REST & WebSocket client integration
│   ├── UniversalSmartHome.Connectors.MCP         # Generic MCP client (Streamable HTTP, stdio, tool execution)
│   ├── UniversalSmartHome.Connectors.Matter      # Matter controller abstraction and device discovery
│   ├── UniversalSmartHome.Api                    # REST API endpoints, DTO contracts, SignalR Hub
│   ├── UniversalSmartHome.Edge                   # Composition root & background hosting service
│   └── UniversalSmartHome.Desktop                # WPF MVVM Desktop Dashboard
└── tests/
    ├── UniversalSmartHome.Domain.Tests           # Unit tests for domain models, capabilities, and commands
    ├── UniversalSmartHome.Application.Tests      # Unit tests for reconciliation and connector manager
    └── UniversalSmartHome.Connectors.Tests       # Unit tests for connector contracts and execution
```

---

## Architectural Principles

- **Protocol-Agnostic Dashboard**: The UI never directly talks to Home Assistant or MCP SDKs; it interacts strictly with normalized domain representations.
- **Controllability Guarantee**: A device is only controllable if it is online and exposes actionable capabilities. Discovery alone never implies controllability.
- **Provider Identity Separation**: Provider-specific entity IDs (e.g., `light.living_room_ceiling`) are tracked separately from internal platform `DeviceId`s.
- **Strict Device Reconciliation**: Devices with the same name are never merged automatically; explicit mappings and hardware identifiers are required.
- **Connector Failure Isolation**: Connectors run independently. A network or authentication failure in one connector does not affect other integrations.

---

## How to Build and Run

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Windows 10/11 (for WPF Desktop application and DPAPI token storage)

### Build
```bash
# Build the entire solution
dotnet build UniversalSmartHome.sln --no-restore -m:1 /nodeReuse:false
```

### Run Tests
```bash
# Run all unit tests
dotnet test UniversalSmartHome.sln --no-restore
```

### Run Edge Host
```bash
dotnet run --project src/UniversalSmartHome.Edge/UniversalSmartHome.Edge.csproj
```

### Run Desktop Application
```bash
dotnet run --project src/UniversalSmartHome.Desktop/UniversalSmartHome.Desktop.csproj
```
