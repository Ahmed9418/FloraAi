# FloraAI Backend API 🌱

FloraAI is a hybrid GenAI plant care system and disease diagnosis platform. This repository contains the RESTful backend API built with ASP.NET Core 8, which serves as the central brain connecting a Flutter mobile application, an on-device computer vision model, and Google's Gemini Large Language Model.

## 🚀 Features

* **Smart Disease Diagnosis:** Receives classification labels from an edge AI model (`.tflite`) and dynamically generates highly personalized, step-by-step treatment plans using Google Gemini.
* **Intelligent Failsafe (Strategy Pattern):** If the GenAI service is offline or times out, the system seamlessly falls back to a hardcoded mathematical watering calculator and predefined Arabic treatment plans without interrupting the user experience.
* **Dynamic Care Schedules:** Automatically calculates future watering and fertilizing dates based on plant species and current health status.
* **Health Record Tracking:** Maintains a timestamped timeline of a plant's health history and past diagnoses.
* **Arabic Localization:** Deeply integrated Arabic language support. English model outputs (e.g., `fungi`, `pests`) are translated natively in the backend, and the AI is prompted to return all UI-facing strings in Arabic.
* **Secure:** JWT-based stateless authentication and BCrypt password hashing.

## 🛠️ Tech Stack

* **Framework:** C# / .NET 8.0 Web API
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **AI Integration:** Google Gemini 1.5/2.5 Flash API
* **Authentication:** JWT Bearer Tokens
* **Documentation:** Swagger / OpenAPI

## ⚙️ Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Visual Studio 2022 (or VS Code)
* SQL Server (e.g., LocalDB or SQLEXPRESS)
* A valid [Google Gemini API Key](https://aistudio.google.com/app/apikey)
