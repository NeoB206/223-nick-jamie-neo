# Sicherheitsüberlegungen für die Architektur

## Ausgangslage
Sie haben mit der L-Bank eine simple Multiuser-Datenbank-Applikation
entworfen. Allerdings wurden da keine Sicherheitsüberlegungen gemacht.
In diesem Auftrag soll einleuchtend gezeigt werden, warum es eine Multitier-Architektur braucht.

Nehmen Sie an, Sie haben eine Applikation wie ein icket-Reservationssystem,
eine Bank, eine Dating-Applikation oder eine Schulverwaltungssoftware. 
Alle diese Applikationen haben folgende [Epics]() gemeinsam:

- Die Benutzer:innen sollen mit einer grafische Benutzerschnittstelle (GUI) arbeiten können.
- Alle Benutzer:innen sollen auf die gleiche Datenbasis Zugriff haben
(«greifen auf die gleiche Datenbank zu»)
- Die Zugriffe können gleichzeitig erfolgen.

## Aufgabenstellung
Bestimmen Sie eine korrekte und vernünftige Architektur.

### Teilaufgabe 1: Hypothese

Laut Aufgabenstellung haben Sie folgende Komponenten:
- Ein Programm, dass das GUI darstellt.
- Eine Business-Logik (dort, wo die Aufgaben erledigt werden: Buchungen in einer Konto-Applikation, Absenzen zählen in einer Schulverwaltung,Matches generieren in einer Dating-Applikation. Also dort «wo die Musik
spielt»).
- Eine Datenbank.
- Da Sie eine Datenbank haben, brauchen Sie noch ein Netzwerk

Ordnen Sie die Komponenten an, wie Sie denken, es würde Sinn ergeben und
verbinden Sie diese

GUI (3 Benutzer:innen):

![](2024-11-22-10-57-26.png)

Business-Logik:

![](2024-11-22-10-57-39.png)

Datenbank:

![](2024-11-22-10-57-46.png)

Es gibt noch eine Entscheidung, die Sie fällen müssen: Wie verbinden Sie sich zur Datenbank?

Sie haben zwei Möglichkeiten:

Variante 1: Jede/r Benutzer:in verbindet sich mit ein und demselben Benutzer auf die Datenbank, Sie programmieren die Authentifizierung und Autorisierung selbst.

Variante 2: Für jede/n Benutzer:in wird in der Datenbank ein eigener User
angelegt. Damit kann die Rechteverwaltung der Datenbank verwendet werden.
Grundsätzlich können Sie für BenutzerInnen die Rechte zum Zugriff auf Tabellen (einfügen, löschen, ändern des Schemas, …) geben

Beantworten Sie folgende Fragen zu Ihrer Hypothese:
- Wie kommen die Daten zur Datenbank? ```Über die GUI zur Business-Logik, die sie verarbeitet und an die Datenbank weiterleitet.```
- Was befindet sich auf den Rechner der Benutzer:innen? ```Die GUI zur Eingabe und Anzeige der Daten.```
- Was befindet sich auf einem Server? ```Die Business-Logik und die Datenbank.```
- Welche Variante zur Verbindung haben Sie gewählt? Variante 1 oder 2? ```Variante 1, da die Business-Logik zentral die Authentifizierung und Autorisierung steuert.```

### Teilaufgabe 2: Das Problem

Eine Möglichkeit wäre, ein Programm mit GUI zu programmieren, bei der die
Businesslogik auch gerade integriert ist, das über das Internet mit der
Datenbank Verbindung aufnimmt

![](2024-11-22-10-59-07.png)

Das Problem mit Variante 1:

Nehmen Sie an, es wird mit Variante 1 gearbeitet: Jede/r Benutzer:in verbindet sich mit ein und demselben Benutzer auf die Datenbank.

Der Username und der Benutzer müssen in der GUI-Applikation gespeichert
werden, sonst kann sie nicht auf die Datenbank zugreifen.

Es gibt nun aber die Möglichkeit, aus den «exe»-Dateien von .net-Programmen
den Quellcode zu gewinnen. Installieren Sie dotPeek5, einer der besten
Dekompilierer.

Laden Sie das Beispielprogramm 2-Tier-Security-Release herunter
und starten Sie es. [2-Tier-Security-Release](./Beispielprogramm/2-Tier-Security-Release.zip)

Starten Sie dotPeek und öffnen Sie die exe-Datei. Sie sehen den Quellcode.

Wählen Sie «2-Tier-Security», «_2_Tier_Security», «Form1» und dann «Form1.cs»

Fällt Ihnen etwas – für die Betreiber der Applikation – unangenehmes auf? Was ziehen Sie daraus für Schlüsse?
1. Fest codierte Verbindungszeichenkette
    - Den Servernamen (LOVIATHAR\\SQLEXPRESS).
    - Den Benutzernamen (administrator).
    - Das Passwort (geheimesPasswort).
2. Fehlende Fehlerbehandlung Fehler werden zwar abgefangen und angezeigt, werden jedoch direkt an den Benutzer weitergegeben, was interne Informationen über die Datenbank oder die Infrastruktur preisgeben könnte (z.B. Datenbankschema oder Fehlermeldungen).
3. Mangel an rollenbasierter Zugriffskontrolle. Der verwendete Benutzer administrator deutet darauf hin, dass dieser Benutzer möglicherweise über umfassende Rechte in der Datenbank verfügt.

Problem mit Variante 2

Nehmen Sie an, es wird mit Variante 2 gearbeitet: Für jede/n Benutzer:in wird in der Datenbank ein eigener User angelegt. In der Applikation wird ein Loginfenster gezeigt und so müssen keine Usernamen und Passworte in der
Applikation gespeichert werden.

Nun gibt es ein neues Problem. Nehmen Sie an, eine Benutzerin möchte etwas buchen:
Natürlich darf diese Person nur von ihrem eigenen Konto buchen! Der angelegte Benutzer muss aber das Änderungsrecht auf diese Tabelle haben, sonst kann gar nicht gebucht werden! Natürlich wird das vom GUI verhindert, aber was würden Sie als kreative/r Hacker:in vorschlagen?
1. Direkter Datenbankzugriff
2. SQL-Injection-Angriff
3. Manipulation des Netzwerkverkehrs
4. Reverse Engineering der Applikation
5. Verwendung von privilegierten Benutzerberechtigungen


### Teilaufgabe 3: Lösung

![Die Lösung](image.png)

Frage: Welche Varianten für Username und Passwort sind denkbar? 
```Zentrale Authentifizierung: Anmeldung über Dienste wie OAuth2, LDAP oder Active Directory. Vorteil: Standardisiert und sicher.```

Wieso haben wir die Probleme bei Variante 1 (jede/r Benutzer:in verbindet sich mit ein und demselben Benutzer auf die Datenbank) nicht?
```Kein direkter Datenbankzugriff: Benutzer:innen greifen nur über die Businesslogik auf die Datenbank zu. Zugangsdaten sind nicht in der GUI gespeichert.```

Wieso haben wir die Probleme bei Variante 2 (für jede/n Benutzer:in wird in der Datenbank ein eigener User angelegt) nicht?
```Keine Rechteprobleme: Benutzer:innen können nur auf die Daten zugreifen, die sie dürfen – gesteuert durch die Businesslogik.```

Schlechte Nachricht: Das wird komplex.

Gute Nachricht: Sie können Ihr Wissen aus dem Modul «295 Backend für Applikationen realisieren» anwenden.