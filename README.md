# Zwitscher
Implementierung von Anwendungssystemen SS23

## Installation
**Bei der Installation und der Nutzung _muss_ eine Verbindung zum Netzwerk der Uni Siegen bestehen!**
### Einrichten der Datenbankverbindung
Rechtsklick auf connected Services>hinzufügen>SQL Server Datenbank>SQL Server Datenbank
Dann folgende Eingaben machen:
Bei Name der Verbindungszeichenfolge: ConnectionStrings:ZwitscherContext
Und beim Wert: Server=ubi30.informatik.uni-siegen.de;Database=Group2;User Id=Group2;Password=**Password**;Encrypt=False

Überprüfen, ob in der Secrets.json folgender Text steht (Passwort muss ersetzt werden):
```
{
  "ConnectionStrings": {
    "ConnectionStrings:ZwitscherContext": "Server=ubi30.informatik.uni-siegen.de;Database=Group2;User Id=Group2;Password=Password;Encrypt=False"
  }
}
```

### Installation des Projektes (MVC und React)
Ordner clientapp > right click > Open in Terminal > type and submit "npm install" > type and submit "npm start" >Starten des MVC Project mit dem Play Knopf oben in Visual Studio > es sollte sich ein Browser Fenster öffnen
