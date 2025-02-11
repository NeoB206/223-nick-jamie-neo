# Authentifizierung

Diese Multiuserapp braucht auch eine Authentifizierung und eine 
Autorisierung.

## Aufgabenstellung
Finden Sie heraus, wie die Authentifizierung und die Autorisierung gelöst
wurden.

## Teilaufgabe 1: Authentifizierung
Beantworten Sie anhand des Quellcodes folgende Fragen:

- Findet eine Authentisierung statt? ```Ja, eine Authentisierung findet statt```
- Wo und wie findet die Authentifizierung statt? ```In der Methode "Authenticate", indem Benutzername und Passwort geprüft werden.```
- Was für einen Typ von Autorisierung verwendet dieses Projekt? ```Rollenbasierte Autorisierung```
- Wie werden die Zugriffe autorisiert? Gibt es in diesem Projekt Capabilities? ```Zugriffe basieren auf Benutzerrollen. Capabilities gibt es nicht.```
- Wo werden den Benutzer:innen die Rollen zugewiesen und wo sind sie gespeichert? ```Rollen werden in "Update" zugewiesen und in der Users-Tabelle gespeichert.```

## Teilaufgabe 2: Authentifizierung

Ändern Sie im Quellcode temporär die Rechte auf die Liste aller Ledger so, dass nur Benutzer:innen in der Rolle «Administrators» Zugriff erhalten.

Benutzen Sie JSON-MAN oder POSTMAN und führen Sie folgende Schritte aus:

1. Rufen Sie die Liste aller Ledgers ohne Authentifizierung auf, was passiert? ```Unauthorized```
2. Loggen Sie sich als ein User mit der korrekten Rolle ein und rufen Sie die
Liste aller Ledgers auf. Was passiert? ```Resultat kommt```
3. Loggen Sie sich als ein User mit einer Rolle ohne Rechte ein und rufen Sie die Liste aller Ledgers auf. Was passiert? ```Unauthorized```