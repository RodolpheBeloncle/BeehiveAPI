# Documentation pour l'application Full-Stack (Vue.js, C#, SQL Server)

## Plan de la Documentation :
1. [Configuration de l'environnement](#1-configuration-de-lenvironnement)
2. [Création de la base de données SQL Server](#2-création-de-la-base-de-données-sql-server)
3. [Création du projet .NET Backend (API)](#3-création-du-projet-net-backend-api)
4. [Création de l'application Vue.js Frontend](#4-création-de-lapplication-vuejs-frontend) voir répertoire Beehive-frontend
5. [Exécution et test de l'application](#5-exécution-et-test-de-lapplication)

---

## 1. Configuration de l'environnement
Avant de commencer, assure-toi d'avoir installé les outils suivants :
- **Visual Studio** (version 2022 recommandée)
  - Installer la charge de travail _Développement web et cloud_ (inclut ASP.NET Core).
- **SQL Server** (ou SQL Server Express) et SQL Server Management Studio (SSMS).
- **Node.js** (inclut npm pour gérer les dépendances front-end).
- **Vue CLI** pour initialiser les projets Vue.js :

 # dotnet_project
api dotnet in mvc architecture

## Mise en place de l'envirronnement
Dans un envirronnement windows une fois que gitbash shell est installer
```bash
nano ~/.bashrc
alias code='"/c/Program Files/Microsoft Visual Studio/2022/Community/Common7/ID>
source ~/.bashrc
code .

```

## mise en place de connexion ssh via power shell
click droit sur powershell execution en tant qu'admin
puis taper la commande :  Add-WindowsCapability -Online -Name OpenSSH.Client*
  
## 2. Création de la base de données SQL Server

##Installation sql server : 

https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16
Download SSMS
Download SQL Server Management Studio (SSMS) 20.2

Crée une nouvelle base de données appelée BeehiveDB pour stocker les données des ruches.

```sql

CREATE DATABASE BeehiveDB;
GO
USE BeehiveDB;

-- Table des ruches
CREATE TABLE Beehives (
    Id INT PRIMARY KEY IDENTITY,
    Location NVARCHAR(100),
    CreatedAt DATETIME
);

-- Table des capteurs
CREATE TABLE Sensors (
    Id INT PRIMARY KEY IDENTITY,
    BeehiveId INT FOREIGN KEY REFERENCES Beehives(Id),
    SensorType NVARCHAR(50),
    Value FLOAT,
    RecordedAt DATETIME
);

```
Table Beehives : stocke les informations sur chaque ruche (emplacement, date de création).
Table Sensors : stocke les données envoyées par les capteurs (type de capteur, valeur, horodatage).


## 3. Création du projet .NET Backend (API)
-> Étape 1 : Créer une nouvelle API ASP.NET Core
Ouvre Visual Studio.
Crée un nouveau projet :
Sélectionne "ASP.NET Core Web API".
Nom du projet : BeehiveAPI.
Sélectionne .NET 7.0 
Clique sur Créer.
-> Étape 2 : Configurer la connexion à SQL Server
Dans appsettings.json, configure la chaîne de connexion à SQL Server :

```json
Copier le code
{
  "ConnectionStrings": {
    "BeehiveDB": "Server=localhost;Database=BeehiveDB;Trusted_Connection=True;"
  },
  "AllowedHosts": "*"
}

```
Installe le package Entity Framework Core pour SQL Server dans le projet backend :

```bash

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```
-> Étape 3 : Créer le modèle de données
Crée un dossier Models et ajoute deux classes : Beehive.cs et Sensor.cs.

```csharp

// Models/Beehive.cs
using System;
using System.Collections.Generic;

namespace BeehiveAPI.Models
{
    public class Beehive
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Sensor> Sensors { get; set; }
    }
}

```
```csharp

// Models/Sensor.cs
using System;

namespace BeehiveAPI.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public int BeehiveId { get; set; }
        public string SensorType { get; set; }
        public float Value { get; set; }
        public DateTime RecordedAt { get; set; }

        public Beehive Beehive { get; set; }
    }
}

```
-> Étape 4 : Créer le contexte de la base de données
Dans un dossier Data, ajoute une classe BeehiveContext.cs pour interagir avec la base de données.

```csharp

// Data/BeehiveContext.cs
using Microsoft.EntityFrameworkCore;
using BeehiveAPI.Models;

namespace BeehiveAPI.Data
{
    public class BeehiveContext : DbContext
    {
        public BeehiveContext(DbContextOptions<BeehiveContext> options) : base(options) { }

        public DbSet<Beehive> Beehives { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
    }
}
Dans Startup.cs ou Program.cs, configure l'Entity Framework pour utiliser SQL Server :
```

```csharp

   // configure the database
   builder.Services.AddDbContext<BeehiveContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("BeehiveContext")));

```
-> Étape 5 : Créer les contrôleurs API
Crée un dossier Controllers et ajoute deux contrôleurs : BeehiveController.cs et SensorController.cs.

```csharp
// Controllers/BeehiveController.cs
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BeehiveAPI.Data;
using BeehiveAPI.Models;

namespace BeehiveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeehiveController : ControllerBase
    {
        private readonly BeehiveContext _context;

        public BeehiveController(BeehiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBeehives()
        {
            var beehives = _context.Beehives.ToList();
            return Ok(beehives);
        }

        [HttpPost]
        public IActionResult CreateBeehive(Beehive beehive)
        {
            beehive.CreatedAt = DateTime.Now;
            _context.Beehives.Add(beehive);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetBeehives), new { id = beehive.Id }, beehive);
        }
    }
}

```
```csharp
// Controllers/SensorController.cs
using Microsoft.AspNetCore.Mvc;
using BeehiveAPI.Data;
using BeehiveAPI.Models;
using System.Linq;

namespace BeehiveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly BeehiveContext _context;

        public SensorController(BeehiveContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RecordSensorData(Sensor sensor)
        {
            sensor.RecordedAt = DateTime.Now;
            _context.Sensors.Add(sensor);
            _context.SaveChanges();
            return Ok(sensor);
        }

        [HttpGet("{beehiveId}")]
        public IActionResult GetSensorData(int beehiveId)
        {
            var sensors = _context.Sensors.Where(s => s.BeehiveId == beehiveId).ToList();
            return Ok(sensors);
        }
    }
}

```
# Gérer le projet back avec git

1. Erreur de permission pour le fichier .vsidx
Le message indique que Git n'a pas accès à un fichier dans le dossier .vs, qui est spécifique à Visual Studio. Ces fichiers n'ont généralement pas besoin d'être suivis par Git.

Pour résoudre ce problème, ajoutez le dossier .vs/ au fichier .gitignore, qui permet d'ignorer certains fichiers ou dossiers.

```bash
echo ".vs/" >> .gitignore
git add .gitignore
git commit -m "Ignore les fichiers de Visual Studio"

```


Cela va ajouter une ligne pour ignorer le dossier .vs/ et commettre la modification dans votre repository.

---

2. Avertissements sur les fins de ligne (LF vers CRLF)
Git vous avertit que les fins de ligne vont être converties de LF (utilisé sous Linux/macOS) en CRLF (utilisé sous Windows). Ce n'est qu'un avertissement, mais si vous souhaitez éviter ces messages, vous pouvez configurer Git pour gérer les fins de ligne automatiquement.

Utilisez cette commande pour que Git gère les conversions de façon automatique, selon le système d'exploitation :

```bash
git config --global core.autocrlf true

```
Cela permettra d'ajuster automatiquement les fins de ligne lorsque vous travaillez sur différents systèmes d'exploitation.
---

3. Erreur de branche en amont non configurée
Git vous signale que la branche actuelle (probablement main) n'a pas de branche "amont" (upstream) sur le dépôt distant. Pour configurer la branche locale main avec la branche distante, utilisez cette commande :

```bash
git push --set-upstream origin main

```
Cela va pousser votre branche locale vers le dépôt distant et lier les deux pour de futurs push.
---


```bash
git push --set-upstream origin master

```

4. Étapes finales
Après avoir poussé la bonne branche (soit main, soit master), votre dépôt distant devrait être mis à jour correctement.



