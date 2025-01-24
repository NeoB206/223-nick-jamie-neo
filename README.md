# 223-ma-app

Willkommen beim Ük 223. Dieses Repository enthält den Code für die Applikation, die wir im Modul 223 erstellen werden, sowie die Lerninhalte. 

Es dient jedoch lediglich als Ablage dieser Informationen. Gewisse Informationen befinden sich auch im Learningview, ebenfalls wird wie gewohnt im Learningview die Dokumentation der Arbeit abgegeben, sowie der Progress dokumentiert.

## Setup

Kopiere `.env.example` in `.env` und passe die Werte an.  
Danach kannst du die Datenbank starten mit `docker compose up -d`.

Beachte auch die READMEs in den einzelnen Projekten beim Setup der Umgebung.

# Dokumentation

**Dokumentation der implementierten Prozess und der Überlegungen:**
- Was war die Herausforderung? (3P)
    1. Aufgabenstellungen waren Teilweise nicht gleich wie der Code.
    2. Testumgebung aufsetzen mit Dependency Injection.
    3. Verbindung zur Datenbank hat am Anfang nicht funktionert.
- Wie wurde die Herausforderung gelöst? (3P)
    1. Nachfragen oder selbst herausfinden.
    2. Von einer Vorlage aus dem Internet eine Fixture übernommen und angepasst.
    3. Datenbank manuell erstellen.
- Was war das Resultat? (3P)
    1. Es konnten Alle Aufgaben gelöst werden.
    2. Die Testumgebung konnte Problemlos ausgeführt werden und die Fixture konnte wiederverwendet werden.
    3. Das Problem war, dass es die Datenbank nicht selbst von der App erstellt wurde und sie somit nicht gefunden hat.

**Dokumentation der Tests und der Testergebnisse:**
- Welche Tests wurden durchgeführt? (3P)
    1. Ledger Delete
    2. Book
    3. Loadtest
- Wie wurden die Tests durchgeführt? (3P)
    1. Ohne Mocks, mit einer Testdatenbank
    2. Positiv, Negativ und Fehlertests
    3. Mithilfe von NBomber
- Was war das Resultat? (3P)
    1. Der Ledger wurde gelöscht.
    2. Die Bookings konnten erfolgreich abgeschlossen werden. Bei den Concurrent Tests wurde auch erfolgreich ein Rollback ausgeführt, wann dies Notwendig war.
    3. Die Lasttests waren erfolgreich und es konnten mehrere Szenarien hintereinander ausgeführt werden. Was man jedoch beachten muss, ist das beim Book Lasttest ein genug grosser Betrag auf dem Konto von Ledger 1 sein muss.